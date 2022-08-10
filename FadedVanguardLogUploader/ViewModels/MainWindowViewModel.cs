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
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ModeCommand { get; }
        public ReactiveCommand<Unit, Unit> DateAscendingCommand { get; }
        public ReactiveCommand<Unit, Unit> DateDescendingCommand { get; }
        public ReactiveCommand<Unit, Unit> LengthAscendingCommand { get; }
        public ReactiveCommand<Unit, Unit> LengthDescendingCommand { get; }
        public ReactiveCommand<Unit, Unit> UserAscendingCommand { get; }
        public ReactiveCommand<Unit, Unit> UserDescendingCommand { get; }
        public ReactiveCommand<Unit, Unit> NameAscendingCommand { get; }
        public ReactiveCommand<Unit, Unit> NameDescendingCommand { get; }
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

        public MainWindowViewModel()
        {
            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(Mode);
            DateAscendingCommand = ReactiveCommand.Create(DateAscending);
            DateDescendingCommand = ReactiveCommand.Create(DateDescending);
            LengthAscendingCommand = ReactiveCommand.Create(LengthAscending);
            LengthDescendingCommand = ReactiveCommand.Create(LengthDescending);
            UserAscendingCommand = ReactiveCommand.Create(UserAscending);
            UserDescendingCommand = ReactiveCommand.Create(UserDescending);
            NameAscendingCommand = ReactiveCommand.Create(NameAscending);
            NameDescendingCommand = ReactiveCommand.Create(NameDescending);
            UseGw2ApiCommand = ReactiveCommand.Create(UseGw2Api);
            ErrorHiddenCommand = ReactiveCommand.Create(ErrorHidden);
            CSVOpenCommand = ReactiveCommand.Create(CSVOpen);
            PageAmountCommand = ReactiveCommand.Create<int>(PageAmount);
            CSVDeleteCommand = ReactiveCommand.Create(CSVDelete);
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
        }

        // Set sorting types
        private void DateAscending()
        {
            App.Settings.SortingType = SortingType.DateAscending;
            List.Sort();
        }
        private void DateDescending()
        {
            App.Settings.SortingType = SortingType.DateDescending;
            List.Sort();
        }
        private void LengthAscending()
        {
            App.Settings.SortingType = SortingType.LengthAscending;
            List.Sort();
        }
        private void LengthDescending()
        {
            App.Settings.SortingType = SortingType.LengthDescending;
            List.Sort();
        }
        private void UserAscending()
        {
            App.Settings.SortingType = SortingType.UserAscending;
            List.Sort();
        }
        private void UserDescending()
        {
            App.Settings.SortingType = SortingType.UserDescending;
            List.Sort();
        }
        private void NameAscending()
        {
            App.Settings.SortingType = SortingType.CharcterAscending;
            List.Sort();
        }
        private void NameDescending()
        {
            App.Settings.SortingType = SortingType.CharcterDescending;
            List.Sort();
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
            List.storageIO.Delete();
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
                    Thread thread = new Thread(() => List.SearchFolder(App.Settings.Path));
                    thread.Start();
                }
            }
        }
    }
}
