using Avalonia;
using Avalonia.Input.Platform;
using Avalonia.Threading;
using DynamicData;
using DynamicData.Kernel;
using EVTCLogUploader.Enums;
using EVTCLogUploader.IO;
using EVTCLogUploader.Models.EVTCList;
using EVTCLogUploader.Models.Responce;
using EVTCLogUploader.Utils;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EVTCLogUploader.ViewModels
{
    public class ListViewModel : ViewModelBase
    {
        public ObservableCollection<ListItem> Items { get; } = new();
        public int FileCount
        {
            get => fileCount;
            set => this.RaiseAndSetIfChanged(ref fileCount, value);
        }
        public int ProgressBarValue
        {
            get => progressBarValue;
            set => this.RaiseAndSetIfChanged(ref progressBarValue, value);
        }
        public int ProgressBarMax
        {
            get => progressBarMax;
            set => this.RaiseAndSetIfChanged(ref progressBarMax, value);
        }
        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        private ConcurrentBag<ListItem> StoredItems = new();
        private List<ListItem> FilteredItems = new();
        internal readonly Filter FilterSettings = new();
        private int fileCount = 0;
        private int progressBarValue = 0;
        private int progressBarMax = 100;
        // Added to debug the constructor getting called twice
        private static int i = 0;

        public readonly StorageIO storageIO;

        // Getting called twice?
        public ListViewModel(StorageIO storageIO)
        {
            i++;
            this.storageIO = storageIO;
            UploadCommand = ReactiveCommand.Create(UploadAsync);
        }

        /// <summary>
        /// Unselects all items in storage and visbile
        /// </summary>
        /// <returns></returns>
        public void UnselectAll()
        {
            StoredItems.AsList().ForEach(item => item.IsSelected = false);

            List<ListItem> temp = Items.AsList();
            temp.ForEach(item => item.IsSelected = false);
            Items.Clear();
            Items.AddRange(temp);
        }

        /// <summary>
        /// Selects all items in storage and visbile
        /// </summary>
        /// <returns></returns>
        public void SelectAll()
        {
            StoredItems.AsList().ForEach(item => item.IsSelected = false);
            List<ListItem> temp = Items.AsList();
            temp.ForEach(item => item.IsSelected = true);
            Items.Clear();
            Items.AddRange(temp);
        }

        /// <summary>
        /// Filters and sorts the current stored items.
        /// </summary>
        /// <param name="date">Date to filter too</param>
        /// <param name="time">If date is present. What time to filter too</param>
        /// <param name="select">Should the items be selected</param>
        /// <returns></returns>
        public void Filter(DateTimeOffset? date = null, TimeSpan? time = null, bool select = false)
        {
            if (date.HasValue)
                FilterSettings.timeOffsetMin = date.Value;
            if (time.HasValue)
                FilterSettings.timeOffsetMin = FilterSettings.timeOffsetMin.Date + time.Value;

            FilteredItems = StoredItems.Where(x => FilterSettings.Predicate(x)).ToList();
            FilteredItems.ForEach(x => x.IsSelected = select);
            FileCount = FilteredItems.Count;
            switch (App.Settings.SortingType)
            {
                case SortingType.Date:
                    FilteredItems.Sort((ListItem x, ListItem y) => y.CreationDate.CompareTo(x.CreationDate));
                    break;
                case SortingType.Length:
                    FilteredItems.Sort((ListItem x, ListItem y) => y.Length.CompareTo(x.Length));
                    break;
                case SortingType.User:
                    FilteredItems.Sort((ListItem x, ListItem y) => y.UserName.CompareTo(x.UserName));
                    break;
                case SortingType.Charcter:
                    FilteredItems.Sort((ListItem x, ListItem y) => y.CharcterName.CompareTo(x.CharcterName));
                    break;
            }
            if (App.Settings.SortingToggle)
                FilteredItems.Reverse();
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                Items.AddRange(FilteredItems);
                return;
            }, DispatcherPriority.MaxValue);
        }

        /// <summary>
        /// Loads the data from storageIO into the visible item
        /// </summary>
        /// <returns></returns>
        public void Load()
        {
            StoredItems = storageIO.GetRecords();

            if (App.Settings.Path == "")
            {
                WipeDatabaseAndItems();
                return;
            }

            if (StoredItems.IsEmpty)
                SearchFolder();
            else
                UpdateFolder();
            Filter();
        }

        public void UpdateFolder()
        {
            IEnumerable<string> files = getFiles(true);
            if (!files.Any())
                return;
            GetItems(files);
        }

        public void SearchFolder()
        {
            WipeDatabaseAndItems();
            IEnumerable<string> files = getFiles(false);
            GetItems(files);
        }

        private async void UploadAsync()
        {
            var popup = new PopupViewModel();
            var uploadlist = StoredItems.Where(x => x.IsSelected).ToList();
            if (uploadlist.Count > 50 || uploadlist.Count == 0)
            {
                popup.Title = "Error: Inavlid amount of files to upload " + uploadlist.Count + "/50";
                await ShowDialog.Handle(popup);
                return;
            }
            uploadlist.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
            ProgressBarMax = uploadlist.Count;

            foreach (ListItem file in uploadlist)
            {
                if (file.UploadUrl == string.Empty)
                {
                    DpsReportResponse? responce = await UploaderHttps.UploadEVTCAsync(file.FullPath);
                    if (responce != null && responce.permalink != null)
                        file.UploadUrl = responce.permalink;
                }
                ProgressBarValue++;
            }
            storageIO.UpdateRecordsURL(uploadlist);
            string result = FormatedUploadListString(uploadlist);
            if (Application.Current != null)
                if (Application.Current.Clipboard != null)
                    await Application.Current.Clipboard.SetTextAsync(result);

            ProgressBarValue = ProgressBarMax;
            popup.Title = result;
            await ShowDialog.Handle(popup);
            ProgressBarValue = 0;
        }

        private string FormatedUploadListString(List<ListItem> uploadlist) 
        {
            List<string> clipborad = new()
            {
                $"Raid Logs {DateTime.Now:D}\n"
            };
            Encounter lastboss = Encounter.Empty;
            foreach (ListItem file in uploadlist)
            {
                if (file.UploadUrl == string.Empty)
                    continue;

                if (lastboss == Encounter.Empty || lastboss != file.Encounter)
                {
                    clipborad.Add($"{file.Encounter}");
                    lastboss = file.Encounter;
                }
                clipborad.Add($"{file.UploadUrl}");
            }
            return string.Join("\n", clipborad.ToArray());
        }

        private void WipeDatabaseAndItems()
        {
            StoredItems.Clear();
            storageIO.WipeDB();
        }

        private IEnumerable<string> getFiles(bool filterAlreadyStored)
        {
            if (filterAlreadyStored)
            {
                return Directory.EnumerateFiles(App.Settings.Path, "*evtc*", SearchOption.AllDirectories)
                    .Where(s =>
                    (s.ToLower().EndsWith(".evtc") ||
                    s.ToLower().EndsWith(".evtc.zip") ||
                    s.ToLower().EndsWith(".zevtc")) &&
                    !StoredItems.Any(val => s.Equals(val.FullPath)));
            }
            return Directory.EnumerateFiles(App.Settings.Path, "*evtc*", SearchOption.AllDirectories)
                .Where(s =>
                s.ToLower().EndsWith(".evtc") ||
                s.ToLower().EndsWith(".evtc.zip") ||
                s.ToLower().EndsWith(".zevtc"));
        }

        private void GetItems(IEnumerable<string> files)
        {
            List<Task> bagTasks = new();
            try
            {
                ProgressBarMax = files.Count();
                foreach (string file in files)
                {
                    bagTasks.Add(Task.Run(() =>
                    {
                        if (File.Exists(file))
                            StoredItems.Add(new(file));
                        ProgressBarValue++;
                    }));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            Task.WaitAll(bagTasks.ToArray());
            storageIO.AddRecords(StoredItems);
            Filter();
            ProgressBarValue = 0;
        }
    }
}
