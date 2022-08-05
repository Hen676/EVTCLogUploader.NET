using Avalonia.Controls;
using Avalonia.Themes.Fluent;
using ReactiveUI;
using System;
using System.Diagnostics;
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
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ModeCommand { get; }
        public ReactiveCommand<Unit, Unit> CSVOpenCommand { get; }
        public ReactiveCommand<Unit, Unit> CSVDeleteCommand { get; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        private TimeSpan? time = null;
        private DateTimeOffset? date = null;
        private bool modeToggle = App.settings.ModeToggle;

        public MainWindowViewModel()
        {
            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(Mode);
            CSVOpenCommand = ReactiveCommand.Create(CSVOpen);
            CSVDeleteCommand = ReactiveCommand.Create(CSVDelete);
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
        }

        private void CSVDelete()
        {
            List.storageIO.Delete();
        }

        private void CSVOpen()
        {
            List.storageIO.OpenCSV();
        }

        private void Close(Window window)
        {
            window.Close();
        }

        private void Mode()
        {
            ModeToggle = !ModeToggle;
            App.settings.ModeToggle = ModeToggle;
            App.Fluent.Mode = App.settings.ModeToggle ? FluentThemeMode.Dark : FluentThemeMode.Light;
        }

        private void Save()
        {
            App.settings.Save();
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
                if (App.settings.Path != "")
                    fileDialog.Directory = App.settings.Path;
                string? responce = await fileDialog.ShowAsync(window);
                if (responce != null)
                {
                    App.settings.Path = responce;
                    App.settings.Save();
                    Thread thread = new Thread(() => List.SearchFolder(App.settings.Path));
                    thread.Start();
                }
            }
        }
    }
}
