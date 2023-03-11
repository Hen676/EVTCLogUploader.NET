using Avalonia.Controls;
using EVTCLogUploader.Enums;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;

namespace EVTCLogUploader.ViewModels
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
        public ObservableCollection<Profession> FilterProfessionList { get; } = new(App.Settings.FilterProfession);
        public ObservableCollection<FileType> FilterFileTypeList { get; } = new(App.Settings.FilterFileType);
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectCommand { get; }
        public ReactiveCommand<Unit, Unit> UnselectCommand { get; }
        public ReactiveCommand<Unit, Unit> ModeCommand { get; }
        public ReactiveCommand<string, Unit> LanguageCommand { get; }
        public ReactiveCommand<Unit, Unit> AscDesToggleCommand { get; }
        public ReactiveCommand<SortingType, Unit> SortCommand { get; }
        public ReactiveCommand<FileType, Unit> FilterFileTypeCommand { get; }
        public ReactiveCommand<Encounter, Unit> FilterEncounterCommand { get; }
        public ReactiveCommand<Profession, Unit> FilterProfCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ErrorHiddenCommand { get; }
        public ReactiveCommand<Unit, Unit> WipeDBCommand { get; }
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        private TimeSpan? time = null;
        private DateTimeOffset? date = null;
        private bool modeToggle = App.Settings.ModeToggle;
        private bool errorFilterToggle = App.Settings.ErrorFilterToggle;
        private bool ascDesToggleHeader = App.Settings.SortingToggle;
        private SortingType sort = App.Settings.SortingType;

        public MainWindowViewModel()
        {
            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            SelectCommand = ReactiveCommand.Create(SelectAll);
            UnselectCommand = ReactiveCommand.Create(UnselectAll);
            ModeCommand = ReactiveCommand.Create(ModeAsync);
            LanguageCommand = ReactiveCommand.Create<string>(ChangeLanguageAsync);

            AscDesToggleCommand = ReactiveCommand.Create(AscDesToggle);
            SortCommand = ReactiveCommand.Create<SortingType>(Sort);
            FilterFileTypeCommand = ReactiveCommand.Create<FileType>(FilterFileType);
            FilterEncounterCommand = ReactiveCommand.Create<Encounter>(FilterEncounter);
            FilterProfCommand = ReactiveCommand.Create<Profession>(FilterProf);
            ClearFilterCommand = ReactiveCommand.Create(ClearFilter);

            ErrorHiddenCommand = ReactiveCommand.Create(ErrorHidden);
            WipeDBCommand = ReactiveCommand.Create(WipeDB);
            CloseCommand = ReactiveCommand.Create<Window>(Close);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
        }

        private void UnselectAll() => List.UnselectAll();
        private void SelectAll() => List.SelectAll();

        private void FilterFileType(FileType fileType)
        {
            if (!FilterFileTypeList.Contains(fileType))
                FilterFileTypeList.Add(fileType);
            else
                FilterFileTypeList.Remove(fileType);
            App.Settings.FilterFileType = new(FilterFileTypeList);
            List.Filter();
        }

        private void FilterProf(Profession profession)
        {
            if (!FilterProfessionList.Contains(profession))
                FilterProfessionList.Add(profession);
            else
                FilterProfessionList.Remove(profession);
            App.Settings.FilterProfession = new(FilterProfessionList);
            List.Filter();
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
        private void FilterEncounter(Encounter encounter)
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

        private void Close(Window window) => window.Close();
        private void Save() => App.Settings.Save();
        private void WipeDB() => List.storageIO.WipeDB();

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

        public async void About()
        {
            var popup = new PopupViewModel
            {
                Title = App.ProgramName,
                Body = string.Format(Resources.Lang.Resources.LNG_About_Version_Colon, App.Version+"\n") +
                string.Format(Resources.Lang.Resources.LNG_About_Creator_Colon, "Hen676\n") +
                string.Format(Resources.Lang.Resources.LNG_About_Repository_Colon, "https://github.com/Hen676/EVTCLogUploader.NET")
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
