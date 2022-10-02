using Avalonia;
using DynamicData;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.IO;
using FadedVanguardLogUploader.Models;
using FadedVanguardLogUploader.Models.Responce;
using FadedVanguardLogUploader.Utils;
using ReactiveUI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.ViewModels
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
        public readonly StorageIO storageIO = new();

        public ListViewModel()
        {
            UploadCommand = ReactiveCommand.Create(UploadAsync);
        }

        public void Load()
        {
            StoredItems = storageIO.GetRecords();
            if (StoredItems.IsEmpty)
                SearchFolder();
            else
                UpdateFolder();
            Filter();
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
                if (file.UploadUrl != string.Empty)
                    continue;
                else
                {
                    DpsReportResponse? responce = await UploaderHttps.UploadEVTCAsync(file.FullPath);
                    if (responce == null || responce.permalink == null)
                        continue;
                    else
                    {
                        file.UploadUrl = responce.permalink;
                    }
                }
                ProgressBarValue++;
            }
            storageIO.UpdateRecordsURL(uploadlist);

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

            var result = string.Join("\n", clipborad.ToArray());
            if (Application.Current != null)
                if (Application.Current.Clipboard != null)
                    await Application.Current.Clipboard.SetTextAsync(result);

            ProgressBarValue = ProgressBarMax;
            popup.Title = result;
            await ShowDialog.Handle(popup);
            ProgressBarValue = 0;
        }

        public void UpdateFolder()
        {
            if (App.Settings.Path == "")
                return;
            IEnumerable<string> files = Directory.EnumerateFiles(App.Settings.Path, "*evtc*", SearchOption.AllDirectories)
                .Where(s =>
                (s.ToLower().EndsWith(".evtc") ||
                s.ToLower().EndsWith(".evtc.zip") ||
                s.ToLower().EndsWith(".zevtc")) &&
                !StoredItems.Any(val => s.Equals(val.FullPath)));
            if (!files.Any())
                return;
            ConcurrentBag<ListItem> temp = new();
            List<Task> bagTasks = new();
            try
            {
                ProgressBarMax = files.Count();
                foreach (string file in files)
                {
                    bagTasks.Add(Task.Run(() =>
                    {
                        ListItem item = new(file);
                        temp.Add(item);
                        StoredItems.Add(item);
                    }));
                    ProgressBarValue++;
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            Task.WaitAll(bagTasks.ToArray());
            storageIO.AddRecords(temp);
            Filter();
            ProgressBarValue = 0;
        }

        public void SearchFolder()
        {
            StoredItems.Clear();
            storageIO.WipeDB();
            if (App.Settings.Path == "")
                return;
            IEnumerable<string> files = Directory.EnumerateFiles(App.Settings.Path, "*.*", SearchOption.AllDirectories)
                .Where(s => s.ToLower().EndsWith(".evtc") || s.ToLower().EndsWith(".evtc.zip") || s.ToLower().EndsWith(".zevtc"));
            List<Task> bagTasks = new();
            try
            {
                ProgressBarMax = files.Count();
                foreach (string file in files)
                {
                    bagTasks.Add(Task.Run(() =>
                    {
                        if (File.Exists(file))
                            StoredItems.Add(new ListItem(file));
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

        public void Sort()
        {
            switch (App.Settings.SortingType)
            {
                case SortingType.Date:
                    FilteredItems.Sort(delegate (ListItem x, ListItem y)
                    {
                        return App.Settings.SortingToggle ? x.CreationDate.CompareTo(y.CreationDate) : y.CreationDate.CompareTo(x.CreationDate);
                    });
                    break;
                case SortingType.Length:
                    FilteredItems.Sort(delegate (ListItem x, ListItem y)
                    {
                        return App.Settings.SortingToggle ? x.Length.CompareTo(y.Length) : y.Length.CompareTo(x.Length);
                    });
                    break;
                case SortingType.User:
                    FilteredItems.Sort(delegate (ListItem x, ListItem y)
                    {
                        return App.Settings.SortingToggle ? x.UserName.CompareTo(y.UserName) : y.UserName.CompareTo(x.UserName);
                    });
                    break;
                case SortingType.Charcter:
                    FilteredItems.Sort(delegate (ListItem x, ListItem y)
                    {
                        return App.Settings.SortingToggle ? x.CharcterName.CompareTo(y.CharcterName) : y.CharcterName.CompareTo(x.CharcterName);
                    });
                    break;
            }
            Items.Clear();
            Items.AddRange(FilteredItems);
        }

        public void Filter(DateTimeOffset? date = null, TimeSpan? time = null)
        {
            if (date.HasValue)
                FilterSettings.timeOffsetMin = date.Value;
            if (time.HasValue)
                FilterSettings.timeOffsetMin = FilterSettings.timeOffsetMin.Date + time.Value;

            FilteredItems = StoredItems.Where(x =>
            {
                if (App.Settings.ErrorFilterToggle && x.Encounter == Encounter.Unkown)
                    return false;
                return FilterSettings.Predicate(x);
            }).ToList();
            FileCount = FilteredItems.Count;
            Sort();
        }
    }
}
