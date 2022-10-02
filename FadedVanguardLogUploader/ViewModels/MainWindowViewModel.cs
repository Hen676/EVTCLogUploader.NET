using Avalonia.Controls;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.IO;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace FadedVanguardLogUploader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ListViewModel List { get; } = new ListViewModel();
        public TimeSpan? Time
        {
            get => time;
            set
            {
                List.Filter(time: value);
                this.RaiseAndSetIfChanged(ref time, value);
            }
        }
        public DateTimeOffset? Date
        {
            get => date;
            set
            {
                List.Filter(date: value);
                this.RaiseAndSetIfChanged(ref date, value);
            }
        }
        public bool ModeToggle
        {
            get => modeToggle;
            set
            {
                this.RaiseAndSetIfChanged(ref modeToggle, value);
            }
        }
        public bool Gw2ApiToggle
        {
            get => gw2ApiToggle;
            set
            {
                this.RaiseAndSetIfChanged(ref gw2ApiToggle, value);
            }
        }
        public bool ErrorFilterToggle
        {
            get => errorFilterToggle;
            set
            {
                this.RaiseAndSetIfChanged(ref errorFilterToggle, value);
            }
        }
        public bool AscDesToggleHeader
        {
            get => ascDesToggleHeader;
            set
            {
                this.RaiseAndSetIfChanged(ref ascDesToggleHeader, value);
            }
        }
        public SortingType SortType
        {
            get => sort;
            set
            {
                this.RaiseAndSetIfChanged(ref sort, value);
            }
        }
        public ObservableCollection<Encounter> FilterList { get; } = new(App.Settings.FilterEncounter);
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }

        public ReactiveCommand<Unit, Unit> ModeCommand { get; }
        public ReactiveCommand<string, Unit> LanguageCommand { get; }

        public ReactiveCommand<Unit, Unit> AscDesToggleCommand { get; }
        public ReactiveCommand<SortingType, Unit> SortCommand { get; }
        public ReactiveCommand<Encounter, Unit> FilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; }
        public ReactiveCommand<Unit, Unit> UseGw2ApiCommand { get; }
        public ReactiveCommand<Unit, Unit> ErrorHiddenCommand { get; }
        public ReactiveCommand<Unit, Unit> WipeDBCommand { get; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        private TimeSpan? time = null;
        private DateTimeOffset? date = null;
        private bool modeToggle = App.Settings.ModeToggle;
        private bool gw2ApiToggle = App.Settings.ApiToggle;
        private bool errorFilterToggle = App.Settings.ErrorFilterToggle;
        private bool ascDesToggleHeader = App.Settings.SortingToggle;
        private SortingType sort = App.Settings.SortingType;

        public MainWindowViewModel()
        {
            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(ModeAsync);
            LanguageCommand = ReactiveCommand.Create<string>(ChangeLanguageAsync);

            AscDesToggleCommand = ReactiveCommand.Create(AscDesToggle);
            SortCommand = ReactiveCommand.Create<SortingType>(Sort);
            FilterCommand = ReactiveCommand.Create<Encounter>(Filter);
            ClearFilterCommand = ReactiveCommand.Create(ClearFilter);

            UseGw2ApiCommand = ReactiveCommand.Create(UseGw2Api);
            ErrorHiddenCommand = ReactiveCommand.Create(ErrorHidden);
            WipeDBCommand = ReactiveCommand.Create(WipeDB);
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
        }

        private void AscDesToggle()
        {
            App.Settings.SortingToggle = AscDesToggleHeader = !App.Settings.SortingToggle;
            List.Filter();
        }
        private void Sort(SortingType type)
        {
            App.Settings.SortingType = SortType = type;
            List.Filter();
        }
        private void Filter(Encounter encounter)
        {
            if (!FilterList.Contains(encounter))
                FilterList.Add(encounter);
            else
                FilterList.Remove(encounter);
            App.Settings.FilterEncounter = new(FilterList);
            List.Filter();
        }
        private void ClearFilter()
        {
            App.Settings.FilterEncounter.Clear();
            List.Filter();
        }
        private void ErrorHidden()
        {
            App.Settings.ErrorFilterToggle = ErrorFilterToggle = !ErrorFilterToggle;
            List.Filter();
        }

        private void Close(Window window)
        {
            window.Close();
        }
        private void WipeDB()
        {
            List.storageIO.WipeDB();
        }
        private void UseGw2Api()
        {
            App.Settings.ApiToggle = Gw2ApiToggle = !Gw2ApiToggle;
            if (Gw2ApiToggle)
                GW2ApiHttps.Init();
        }


        private async void ModeAsync()
        {
            App.Settings.ModeToggle = ModeToggle = !ModeToggle;
            var popup = new PopupViewModel
            {
                Title = Resources.Lang.Resources.LNG_Restart_Theme_Title,
                Body = string.Format(Resources.Lang.Resources.LNG_Restart_Theme_Body, ModeToggle)
            };
            await ShowDialog.Handle(popup);
        }
        private async void ChangeLanguageAsync(string code)
        {
            if (App.Settings.Lang == code)
                return;
            App.Settings.Lang = code; ;
            var popup = new PopupViewModel
            {
                Title = Resources.Lang.Resources.LNG_Restart_Language_Title,
                Body = string.Format(Resources.Lang.Resources.LNG_Restart_Language_Body, code)
            };
            await ShowDialog.Handle(popup);
        }

        private void Save()
        {
            App.Settings.Save();
        }

        public async void About()
        {
            var popup = new PopupViewModel
            {
                Title = "Faded Vanguard Log Uploader\n\n" +
                "Version: 1.0.0\n" +
                "Creator: Hen676\n" +
                "Repository: https://github.com/Hen676/FadedDiscordBot.NET"
            };
            await ShowDialog.Handle(popup);
        }

        public async void Folder(Window window)
        {
            var fileDialog = new OpenFolderDialog();
            if (fileDialog != null && window != null)
            {
                fileDialog.Title = "Open Folder";
                if (App.Settings.Path != "")
                    fileDialog.Directory = App.Settings.Path;
                string? responce = await fileDialog.ShowAsync(window);
                if (responce != null)
                {
                    App.Settings.Path = responce;
                    App.Settings.Save();
                    Thread thread = new(() => List.SearchFolder());
                    thread.Start();
                }
            }
        }
    }
}
