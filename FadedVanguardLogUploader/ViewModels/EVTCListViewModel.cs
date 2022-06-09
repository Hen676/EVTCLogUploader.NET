using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using DynamicData;
using FadedVanguardLogUploader.Https;
using FadedVanguardLogUploader.Models;
using FadedVanguardLogUploader.Models.Responce;
using FadedVanguardLogUploader.Util;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace FadedVanguardLogUploader.ViewModels
{
    public class EVTCListViewModel : ViewModelBase
    {
        public ObservableCollection<ListEVTC> Items { get; } = new();
        public int Page
        {
            get => page;
            set => this.RaiseAndSetIfChanged(ref page, value);
        }
        public bool EnabledDown
        {
            get => enabledDown;
            set => this.RaiseAndSetIfChanged(ref enabledDown, value);
        }
        public bool EnabledUp
        {
            get => enabledUp;
            set => this.RaiseAndSetIfChanged(ref enabledUp, value);
        }
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
        public ReactiveCommand<Unit, Unit> PageUpCommand { get; }
        public ReactiveCommand<Unit, Unit> PageDownCommand { get; }
        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        private List<ListEVTC> StoredItems = new();
        private readonly Filter FilterSettings = new();
        private readonly int maxPage = 25;
        private int page = 0;
        private bool enabledDown = false;
        private bool enabledUp = false;
        private int fileCount = 0;
        private int progressBarValue = 0;
        private int progressBarMax = 100;

        //TODO:: EVTCListViewModel created twice?
        public EVTCListViewModel()
        {
            SearchFolder(App.settings.Path);

            PageUpCommand = ReactiveCommand.Create(PageUp);
            PageDownCommand = ReactiveCommand.Create(PageDown);
            UploadCommand = ReactiveCommand.Create(UploadAsync);
        }

        private async void UploadAsync()
        {
            var popup = new PopupViewModel();
            var uploadlist = StoredItems.Where(x => x.IsSelected).ToList();
            if (uploadlist.Count > 50 || uploadlist.Count == 0)
            {
                popup.Message = "Error: Inavlid amount of files to upload " + uploadlist.Count + "/50";
                await ShowDialog.Handle(popup);
                return;
            }
            ProgressBarMax = uploadlist.Count;

            List<DpsReportResponse> responses = new();

            foreach (ListEVTC file in uploadlist)
            {
                if (!file.IsSelected)
                    continue;
                DpsReportResponse? responce = await Uploader.UploadEVTCAsync(file.FileInfo);
                if (responce == null || responce.permalink == null)
                    continue;
                else
                    responses.Add(responce);
                ProgressBarValue++;
            }

            List<string> clipborad = new();
            clipborad.Add($"Raid Logs {DateTime.Now:D}\n");
            responses.Sort((x, y) => x.encounterTime - y.encounterTime);
            string bossname = "";
            foreach (DpsReportResponse responce in responses)
            {
                if (responce.encounter == null)
                    continue;
                if (bossname.Equals("") || bossname.Equals(responce.encounter.boss))
                    clipborad.Add($"{responce.encounter.boss}");

                clipborad.Add(responce.encounter.success ? $"{responce.permalink} (Kill)" : $"{responce.permalink}");
            }

            var result = string.Join("\n", clipborad.ToArray());
            if (Application.Current != null)
                if (Application.Current.Clipboard != null)
                    await Application.Current.Clipboard.SetTextAsync(result);

            ProgressBarValue = ProgressBarMax;
            popup.Message = result;
            await ShowDialog.Handle(popup);
            ProgressBarValue = 0;
        }

        private void PageUp()
        {
            Page++;
            int total = Page * maxPage;
            EnabledDown = true;
            Items.Clear();
            if (StoredItems.Count <= total + maxPage)
            {
                EnabledUp = false;
                Items.AddRange(StoredItems.GetRange(total, StoredItems.Count - total));
            }
            else
                Items.AddRange(StoredItems.GetRange(total, maxPage));
        }

        private void PageDown()
        {
            if (Page == 1)
                EnabledDown = false;
            EnabledUp = true;
            Page--;
            Items.Clear();
            Items.AddRange(StoredItems.GetRange(Page * maxPage, maxPage));
        }

        public void SearchFolder(string path)
        {
            StoredItems.Clear();
            if (path == "/")
                return;
            IEnumerable<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(s => s.ToLower().EndsWith(".evtc") || s.ToLower().EndsWith(".evtc.zip") || s.ToLower().EndsWith(".zevtc"));
            try
            {
                foreach (string file in files)
                {
                    if (File.Exists(file))
                        StoredItems.Add(new ListEVTC(file));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            StoredItems = StoredItems.FindAll(FilterSettings.Predicate);
            FileCount = StoredItems.Count;
            Items.Clear();
            Page = 0;
            EnabledDown = false;
            EnabledUp = StoredItems.Count >= maxPage;
            if (StoredItems.Count == 0)
                return;
            Items.AddRange(StoredItems.GetRange(0, Math.Min(StoredItems.Count, maxPage)));
        }
        public void Filter(DateTimeOffset? date = null, TimeSpan? time = null)
        {
            if (date.HasValue)
                FilterSettings.timeOffsetMin = date.Value;
            if (time.HasValue)
                FilterSettings.timeOffsetMin = FilterSettings.timeOffsetMin.Date + time.Value;

            SearchFolder(App.settings.Path);
        }
    }
}
