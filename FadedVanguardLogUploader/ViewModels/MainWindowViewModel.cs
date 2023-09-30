using Avalonia.Controls;
using EVTCLogUploader.Enums;
using EVTCLogUploader.Services.IO;
using EVTCLogUploader.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Threading;
using EVTCLogUploader.Models.EVTCList;
using EVTCLogUploader.Models.Responce;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using DynamicData.Kernel;
using DynamicData;
using Avalonia;

namespace EVTCLogUploader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();
        #region Commands
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
        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        #endregion
        public ObservableCollection<EVTCFile> Items { get; } = new();
        public int FileCount
        {
            get => _fileCount;
            set => this.RaiseAndSetIfChanged(ref _fileCount, value);
        }
        public int ProgressBarValue
        {
            get => _progressBarValue;
            set => this.RaiseAndSetIfChanged(ref _progressBarValue, value);
        }
        public int ProgressBarMax
        {
            get => _progressBarMax;
            set => this.RaiseAndSetIfChanged(ref _progressBarMax, value);
        }

        private List<EVTCFile> _storedItems = new();
        private List<EVTCFile> _filteredItems = new();
        private int _fileCount = 0;
        private int _progressBarValue = 0;
        private int _progressBarMax = 100;

        #region Serivces
        private IUploaderService _uploaderService;
        private ISettingService _settingService;
        private ILocalDatabaseService _localDatabaseService;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            // Only use in debug check
            _uploaderService = new UploaderService();
            _settingService = new SettingService();
            _localDatabaseService = new LocalDatabaseService();

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

            UploadCommand = ReactiveCommand.Create(UploadAsync);
        }

        public MainWindowViewModel(IUploaderService uploaderService, ISettingService settingService, ILocalDatabaseService localDatabaseService)
        {
            _uploaderService = uploaderService;
            _settingService = settingService;
            _localDatabaseService = localDatabaseService;

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

            UploadCommand = ReactiveCommand.Create(UploadAsync);
        }
        #endregion

        private void FilterFileType(FileType fileType)
        {
            _settingService.FilterSettings.EditFileTypeList(fileType);
            Filter();
        }
        private void FilterEncounter(Encounter encounter)
        {
            _settingService.FilterSettings.EditEncounterList(encounter);
            Filter();
        }
        private void FilterProf(Profession profession)
        {
            _settingService.FilterSettings.EditProfessionList(profession);
            Filter();
        }

        private void AscDesToggle()
        {
            _settingService.SortingToggle = !_settingService.SortingToggle;
            Filter();
        }

        private void Sort(SortingType type)
        {
            _settingService.SortingType = type;
            Filter();
        }

        private void ClearFilter()
        {
            _settingService.FilterSettings.ClearFilters();
            Filter();
        }
        private void ErrorHidden()
        {
            _settingService.FilterSettings.ErrorFilter = !_settingService.FilterSettings.ErrorFilter;
            Filter();
        }

        private void Close(Window window) => window.Close();
        private void Save() => _settingService.Save();
        private void WipeDB() => _localDatabaseService.WipeDB();

        private async void ModeAsync()
        {
            _settingService.ModeToggle = !_settingService.ModeToggle;
            var popup = new PopupViewModel
            {
                Title = Resources.Lang.Resources.LNG_Restart_Theme_Title,
                Body = string.Format(Resources.Lang.Resources.LNG_Restart_Theme_Body, _settingService.ModeToggle)
            };
            await ShowDialog.Handle(popup);
        }
        private async void ChangeLanguageAsync(string code)
        {
            _settingService.SetLanguage(code);
            var popup = new PopupViewModel
            {
                Title = Resources.Lang.Resources.LNG_Restart_Language_Title,
                Body = string.Format(Resources.Lang.Resources.LNG_Restart_Language_Body, code)
            };
            await ShowDialog.Handle(popup);
        }

        private async void About()
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

        private async void Folder(Window window)
        {
            // TODO:: Update folder popup + Add translation
            var fileDialog = new OpenFolderDialog();
            if (fileDialog != null && window != null)
            {
                fileDialog.Title = "Open Folder";
                if (_settingService.Path != "")
                    fileDialog.Directory = _settingService.Path;
                string? responce = await fileDialog.ShowAsync(window);
                if (responce != null)
                {
                    _settingService.Path = responce;
                    _settingService.Save();
                    Thread thread = new(() => SearchFolder());
                    thread.Start();
                }
            }
        }

        /// <summary>
        /// Unselects all items in storage and visbile
        /// </summary>
        /// <returns></returns>
        private void UnselectAll()
        {
            _storedItems.AsList().ForEach(item => item.IsSelected = false);

            List<EVTCFile> temp = Items.AsList();
            temp.ForEach(item => item.IsSelected = false);
            Items.Clear();
            Items.AddRange(temp);
        }

        /// <summary>
        /// Selects all items in storage and visbile
        /// </summary>
        /// <returns></returns>
        private void SelectAll()
        {
            _storedItems.AsList().ForEach(item => item.IsSelected = false);
            List<EVTCFile> temp = Items.AsList();
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
        private void Filter(DateTimeOffset? date = null, TimeSpan? time = null, bool select = false)
        {
            if (date.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = date.Value;
            if (time.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = _settingService.FilterSettings.TimeOffsetMin.Date + time.Value;

            _filteredItems = _storedItems.Where(x => _settingService.FilterSettings.Predicate(x)).ToList();
            _filteredItems.ForEach(x => x.IsSelected = select);
            FileCount = _filteredItems.Count;
            switch (_settingService.SortingType)
            {
                case SortingType.Date:
                    _filteredItems.Sort((EVTCFile x, EVTCFile y) => y.CreationDate.CompareTo(x.CreationDate));
                    break;
                case SortingType.Length:
                    _filteredItems.Sort((EVTCFile x, EVTCFile y) => y.Length.CompareTo(x.Length));
                    break;
                case SortingType.User:
                    _filteredItems.Sort((EVTCFile x, EVTCFile y) => y.UserName.CompareTo(x.UserName));
                    break;
                case SortingType.Charcter:
                    _filteredItems.Sort((EVTCFile x, EVTCFile y) => y.CharcterName.CompareTo(x.CharcterName));
                    break;
            }
            if (_settingService.SortingToggle)
                _filteredItems.Reverse();
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                Items.AddRange(_filteredItems);
                return;
            }, DispatcherPriority.MaxValue);
        }

        /// <summary>
        /// Loads the data from storageIO into the visible item
        /// </summary>
        /// <returns></returns>
        public async void Load()
        {
            _storedItems = await _localDatabaseService.GetRecords();

            if (_settingService.Path == "")
            {
                WipeDatabaseAndItems();
                return;
            }

            if (_storedItems.Count == 0)
                SearchFolder();
            else
                UpdateFolder();
            Filter();
        }

        private void UpdateFolder()
        {
            IEnumerable<string> files = GetFiles(true);
            if (!files.Any())
                return;
            GetItems(files);
        }

        private void SearchFolder()
        {
            WipeDatabaseAndItems();
            IEnumerable<string> files = GetFiles(false);
            GetItems(files);
        }

        private async void UploadAsync()
        {
            var popup = new PopupViewModel();
            var uploadlist = _storedItems.Where(x => x.IsSelected).ToList();
            if (uploadlist.Count > 50 || uploadlist.Count == 0)
            {
                popup.Title = "Error: Inavlid amount of files to upload " + uploadlist.Count + "/50";
                await ShowDialog.Handle(popup);
                return;
            }
            uploadlist.Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
            ProgressBarMax = uploadlist.Count;

            foreach (EVTCFile file in uploadlist)
            {
                if (file.UploadUrl == string.Empty)
                {
                    DpsReportResponse? responce = await _uploaderService.UploadEVTCAsync(file.FullPath);
                    if (responce != null && responce.permalink != null)
                        file.UploadUrl = responce.permalink;
                }
                ProgressBarValue++;
            }
            _localDatabaseService.UpdateRecordsURL(uploadlist);
            string result = FormatedUploadListString(uploadlist);
            if (Application.Current != null)
                //if (Application.Current.Get != null)
                    // await Application.Current.Clipboard.SetTextAsync(result);

            ProgressBarValue = ProgressBarMax;
            popup.Title = result;
            await ShowDialog.Handle(popup);
            ProgressBarValue = 0;
        }

        private string FormatedUploadListString(List<EVTCFile> uploadlist)
        {
            List<string> clipborad = new()
            {
                $"Raid Logs {DateTime.Now:D}\n"
            };
            Encounter lastboss = Encounter.Empty;
            foreach (EVTCFile file in uploadlist)
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
            _storedItems.Clear();
            _localDatabaseService.WipeDB();
        }

        private IEnumerable<string> GetFiles(bool filterAlreadyStored)
        {
            if (filterAlreadyStored)
            {
                return Directory.EnumerateFiles(_settingService.Path, "*evtc*", SearchOption.AllDirectories)
                    .Where(s =>
                    (s.ToLower().EndsWith(".evtc") ||
                    s.ToLower().EndsWith(".evtc.zip") ||
                    s.ToLower().EndsWith(".zevtc")) &&
                    !_storedItems.Any(val => s.Equals(val.FullPath)));
            }
            return Directory.EnumerateFiles(_settingService.Path, "*evtc*", SearchOption.AllDirectories)
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
                            _storedItems.Add(new(file));
                        ProgressBarValue++;
                    }));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            Task.WaitAll(bagTasks.ToArray());
            _localDatabaseService.AddRecords(_storedItems);
            Filter();
            ProgressBarValue = 0;
        }
    }
}
