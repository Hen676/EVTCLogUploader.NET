using Avalonia.Controls;
using Avalonia.Themes.Fluent;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.IO;
using ReactiveUI;
using System;
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
        public string AscDesToggleHeader
        {
            get => ascDesToggleHeader;
            set
            {
                this.RaiseAndSetIfChanged(ref ascDesToggleHeader, value);
            }
        }
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ModeCommand { get; }

        public ReactiveCommand<Unit, Unit> AscDesToggleCommand { get; }
        public ReactiveCommand<Unit, Unit> DateSortCommand { get; }
        public ReactiveCommand<Unit, Unit> LengthSortCommand { get; }
        public ReactiveCommand<Unit, Unit> UserSortCommand { get; }
        public ReactiveCommand<Unit, Unit> NameSortCommand { get; }

        public ReactiveCommand<Unit, Unit> UseGw2ApiCommand { get; }
        public ReactiveCommand<Unit, Unit> ErrorHiddenCommand { get; }
        public ReactiveCommand<Unit, Unit> CSVOpenCommand { get; }
        public ReactiveCommand<int, Unit> PageAmountCommand { get; }
        public ReactiveCommand<Unit, Unit> CSVDeleteCommand { get; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        private TimeSpan? time = null;
        private DateTimeOffset? date = null;
        private bool modeToggle = App.Settings.ModeToggle;
        private bool gw2ApiToggle = App.Settings.ApiToggle;
        private bool errorFilterToggle = App.Settings.ErrorFilterToggle;
        private string ascDesToggleHeader = "Error";

        public MainWindowViewModel()
        {
            AscDesToggleHeader = AscDesToggleString(App.Settings.SortingToggle);

            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(Mode);

            AscDesToggleCommand = ReactiveCommand.Create(AscDesToggle);

            DateSortCommand = ReactiveCommand.Create(DateSort);
            LengthSortCommand = ReactiveCommand.Create(LengthSort);
            UserSortCommand = ReactiveCommand.Create(UserSort);
            NameSortCommand = ReactiveCommand.Create(NameSort);

            UseGw2ApiCommand = ReactiveCommand.Create(UseGw2Api);
            ErrorHiddenCommand = ReactiveCommand.Create(ErrorHidden);
            CSVOpenCommand = ReactiveCommand.Create(CSVOpen);
            PageAmountCommand = ReactiveCommand.Create<int>(PageAmount);
            CSVDeleteCommand = ReactiveCommand.Create(CSVDelete);
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
        }

        private void AscDesToggle()
        {
            App.Settings.SortingToggle = !App.Settings.SortingToggle;
            AscDesToggleHeader = AscDesToggleString(App.Settings.SortingToggle);
            List.Filter();
        }

        private static string AscDesToggleString(bool toggle)
        {
            return toggle ? "\u2191 Ascending" : "\u2193 Descending";
        }

        // Set sorting types
        // TODO:: Refine to simple sort types, remove asc/des from enum
        private void DateSort()
        {
            App.Settings.SortingType = SortingType.Date;
            List.Filter();
        }
        private void LengthSort()
        {
            App.Settings.SortingType = SortingType.Length;
            List.Filter();
        }
        private void UserSort()
        {
            App.Settings.SortingType = SortingType.User;
            List.Filter();
        }
        private void NameSort()
        {
            App.Settings.SortingType = SortingType.Charcter;
            List.Filter();
        }


        private void Close(Window window)
        {
            window.Close();
        }
        private void PageAmount(int pageAmount)
        {
            App.Settings.PageAmount = pageAmount;
            List.Filter();
        }
        private void CSVDelete()
        {
            List.storageIO.WipeDB();
        }
        private void CSVOpen()
        {
            List.storageIO.OpenCSV();
        }
        private void UseGw2Api()
        {
            Gw2ApiToggle = !Gw2ApiToggle;
            App.Settings.ApiToggle = Gw2ApiToggle;
            if (Gw2ApiToggle)
                GW2ApiHttps.Init();
        }
        private void ErrorHidden()
        {
            ErrorFilterToggle = !ErrorFilterToggle;
            App.Settings.ErrorFilterToggle = ErrorFilterToggle;
            List.Filter();
        }

        private void Mode()
        {
            ModeToggle = !ModeToggle;
            App.Settings.ModeToggle = ModeToggle;
            App.Fluent.Mode = App.Settings.ModeToggle ? FluentThemeMode.Dark : FluentThemeMode.Light;
        }

        private void Save()
        {
            App.Settings.Save();
        }

        public async void About()
        {
            var popup = new PopupViewModel
            {
                Message = "Faded Vanguard Log Uploader\n\n" +
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
