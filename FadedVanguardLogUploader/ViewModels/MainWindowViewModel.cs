using Avalonia.Controls;
using EVTCLogUploader.Enums;
using EVTCLogUploader.Services;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using Avalonia.Threading;
using EVTCLogUploader.Models.Responce;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using DynamicData;
using Avalonia.Styling;
using EVTCLogUploader.Utils.Determiners;
using EVTCLogUploader.Models;
using System.Collections.Specialized;
using Avalonia.Platform.Storage;
using Splat;

namespace EVTCLogUploader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public Interaction<PopupViewModel, bool> ShowDialog { get; } = new Interaction<PopupViewModel, bool>();

        #region Commands
        public ReactiveCommand<Unit, Unit> AboutCommand { get; }
        public ReactiveCommand<Unit, Unit> SaveCommand { get; }
        public ReactiveCommand<Unit, Unit> ModeCommand { get; }
        public ReactiveCommand<string, Unit> LanguageCommand { get; }
        public ReactiveCommand<FileType, Unit> FilterFileTypeCommand { get; }
        public ReactiveCommand<Encounter, Unit> FilterEncounterCommand { get; }
        public ReactiveCommand<Profession, Unit> FilterProfCommand { get; }
        public ReactiveCommand<Unit, Unit> ClearFilterCommand { get; }
        public ReactiveCommand<Unit, Unit> ErrorHiddenCommand { get; }
        public ReactiveCommand<Unit, Unit> WipeDBCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        public ReactiveCommand<Window, Unit> UploadCommand { get; }
        public ReactiveCommand<Unit, Unit> HideFilterCommand { get; }
        #endregion

        #region Observables
        public ObservableCollection<EVTCFile> Items { get; } = new();
        public ObservableCollection<EncounterNode> FilterNodes { get; } = new()
        {
            new EncounterNode(Resources.Lang.Resources.LNG_Menu_Filter_Raids, new ObservableCollection<EncounterNode>{
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W1, new ObservableCollection<EncounterNode>{
                     new EncounterNode(Encounter.ValeGuardian),
                     new EncounterNode(Encounter.Gorseval),
                     new EncounterNode(Encounter.Sabetha)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W2, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.Slothasor),
                    new EncounterNode(Encounter.BanditTrio),
                    new EncounterNode(Encounter.Mattias)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W3, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.Escort),
                    new EncounterNode(Encounter.KeepConstruct),
                    new EncounterNode(Encounter.TwistedCastle),
                    new EncounterNode(Encounter.Xera)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W4, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.Cairn),
                    new EncounterNode(Encounter.MursaatOverseer),
                    new EncounterNode(Encounter.Samarog),
                    new EncounterNode(Encounter.Deimos)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W5, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.SoullessHorror),
                    new EncounterNode(Encounter.RiverOfSouls),
                    new EncounterNode(Encounter.BrokenKing),
                    new EncounterNode(Encounter.EaterOfSouls),
                    new EncounterNode(Encounter.Eyes),
                    new EncounterNode(Encounter.Dhuum)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W6, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.ConjuredAmalgamate),
                    new EncounterNode(Encounter.TwinLargos),
                    new EncounterNode(Encounter.Qadim)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_W7, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.Adina),
                    new EncounterNode(Encounter.Sabir),
                    new EncounterNode(Encounter.QadimThePeerless)
                }),
            }),
            new EncounterNode(Resources.Lang.Resources.LNG_Menu_Filter_Strikes, new ObservableCollection<EncounterNode>{
                new EncounterNode(Encounter.Freezie),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_LW5, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.ShiverpeaksPass),
                    new EncounterNode(Encounter.VoiceAndClawOfTheFallen),
                    new EncounterNode(Encounter.FraenirOfJormag),
                    new EncounterNode(Encounter.Boneskinner),
                    new EncounterNode(Encounter.WhisperOfJormag),
                    new EncounterNode(Encounter.ForgingSteel),
                    new EncounterNode(Encounter.ColdWar)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_EOD, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.AetherbladeHideout),
                    new EncounterNode(Encounter.XunlaiJadeJunkyard),
                    new EncounterNode(Encounter.KainengOverlook),
                    new EncounterNode(Encounter.HarvestTemple)
                }),
                new EncounterNode(Encounter.OldLionsCourt),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_SOFO, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.CosmicObservatory),
                    new EncounterNode(Encounter.TempleOfFebe)
                })
            }),
            new EncounterNode(Resources.Lang.Resources.LNG_Menu_Filter_Fractels, new ObservableCollection<EncounterNode>{
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_Nightmare, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.MAMA),
                    new EncounterNode(Encounter.Siax),
                    new EncounterNode(Encounter.Ensolyss)
                }),
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_ShatteredObservatory, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.Skorvald),
                    new EncounterNode(Encounter.Artsariiv),
                    new EncounterNode(Encounter.Arkk)
                }),
                new EncounterNode(Encounter.AiKeeperOfThePeak),
                new EncounterNode(Encounter.Kanaxai)
            }),
            new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_Golems, new ObservableCollection<EncounterNode>{ 
                new EncounterNode(Encounter.StandardKittyGolem),
                new EncounterNode(Encounter.MediumKittyGolem),
                new EncounterNode(Encounter.LargeKittyGolem),
                new EncounterNode(Encounter.MassiveKittyGolem)
            })
        };
        public ObservableCollection<EncounterNode> SelectedFilterNodes { get; } = new();
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
        public ThemeVariant Theme
        {
            get => _theme;
            set => this.RaiseAndSetIfChanged(ref _theme, value);
        }
        public bool FilterError
        {
            get => _filterError;
            set => this.RaiseAndSetIfChanged(ref _filterError, value);
        }
        public bool HideFilters
        {
            get => _hideFilters;
            set => this.RaiseAndSetIfChanged(ref _hideFilters, value);
        }
        public DateTimeOffset? DateFrom
        {
            get => _dateFrom;
            set
            {
                Filter(dateFrom: value);
                this.RaiseAndSetIfChanged(ref _dateFrom, value);
            }
        }
        public DateTimeOffset? DateTo
        {
            get => _dateTo;
            set
            {
                Filter(dateTo: value);
                this.RaiseAndSetIfChanged(ref _dateTo, value);
            }
        }
        public TimeSpan? TimeFrom
        {
            get => _timeFrom;
            set
            {
                Filter(timeFrom: value);
                this.RaiseAndSetIfChanged(ref _timeFrom, value);
            }
        }
        public TimeSpan? TimeTo
        {
            get => _timeTo;
            set
            {
                Filter(timeTo: value);
                this.RaiseAndSetIfChanged(ref _timeTo, value);
            }
        }
        #endregion

        #region Private
        private List<EVTCFile> _storedItems = new();
        private List<EVTCFile> _filteredItems = new();
        private int _fileCount = 0;
        private int _progressBarValue = 0;
        private int _progressBarMax = 100;
        private ThemeVariant _theme;
        private bool _filterError;
        private DateTimeOffset? _dateFrom;
        private DateTimeOffset? _dateTo;
        private TimeSpan? _timeFrom;
        private TimeSpan? _timeTo;
        private bool _hideFilters = true;

        private IUploaderService _uploaderService;
        private ISettingService _settingService;
        private ILocalDatabaseService _localDatabaseService;
        #endregion

        #region Constructor
        public MainWindowViewModel()
        {
            // Only use in debug check
            _uploaderService = Locator.Current.GetService<IUploaderService>() ?? new UploaderService();
            _settingService = Locator.Current.GetService<ISettingService>() ?? new SettingService();
            _localDatabaseService = Locator.Current.GetService<ILocalDatabaseService>() ?? new LocalDatabaseService();

            _theme = ThemeVarientDeterminer.Result(_settingService.ModeToggle);
            _filterError = _settingService.FilterSettings.ErrorFilter;

            SelectedFilterNodes.CollectionChanged += SelectedFilterNodes_CollectionChanged;

            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(ThemeVarient);
            LanguageCommand = ReactiveCommand.Create<string>(ChangeLanguageAsync);

            HideFilterCommand = ReactiveCommand.Create(HideFilter);
            FilterFileTypeCommand = ReactiveCommand.Create<FileType>(FilterFileType);
            FilterEncounterCommand = ReactiveCommand.Create<Encounter>(FilterEncounter);
            FilterProfCommand = ReactiveCommand.Create<Profession>(FilterProf);
            ClearFilterCommand = ReactiveCommand.Create(ClearFilter);

            ErrorHiddenCommand = ReactiveCommand.Create(ErrorHidden);
            WipeDBCommand = ReactiveCommand.Create(WipeDB);
            FolderCommand = ReactiveCommand.Create<Window>(Folder);
            UploadCommand = ReactiveCommand.Create<Window>(UploadAsync);
        }
        #endregion

        #region Filter Commands
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
        private void ClearFilter()
        {
            _settingService.FilterSettings.ClearFilters();
            DateFrom = null;
            DateTo = null;
            TimeFrom = null;
            TimeTo = null;
            SelectedFilterNodes.Clear(); 
            Filter();
        }
        private void ErrorHidden()
        {
            _settingService.FilterSettings.ErrorFilter = !_settingService.FilterSettings.ErrorFilter;
            FilterError = _settingService.FilterSettings.ErrorFilter;
            Filter();
        }
        private void HideFilter()
        {
            HideFilters = !HideFilters;
            if (HideFilters) {
                DateTo = null;
                TimeTo = null;
            }
        }
        #endregion


        private void SelectedFilterNodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<EncounterNode> list && list != null)
            {
                _settingService.FilterSettings.EditEncounterList(list.ToList());
                Filter();
            }
        }

        public void Save() => _settingService.Save();
        private void WipeDB() => _localDatabaseService.WipeDB();

        /// <summary>
        /// Switches the current applications theme from light to dark and vice versa
        /// </summary>
        /// <returns></returns>
        private void ThemeVarient()
        {
            _settingService.ModeToggle = !_settingService.ModeToggle;
            Theme = ThemeVarientDeterminer.Result(_settingService.ModeToggle);
        }

        /// <summary>
        /// Changes the current language of the application. Language chages apply when the application is restarted.
        /// </summary>
        /// <param name="code">Language code to change to</param>
        /// <returns></returns>
        private async void ChangeLanguageAsync(string code)
        {
            bool goodCode = _settingService.SetLanguage(code);
            PopupViewModel popup;
            if (goodCode)
            {
                popup = new PopupViewModel
                {
                    Title = Resources.Lang.Resources.LNG_Restart_Language_Title,
                    Body = string.Format(Resources.Lang.Resources.LNG_Restart_Language_Body, code)
                };
            }
            else 
            {
                popup = new PopupViewModel
                {
                    Title = Resources.Lang.Resources.LNG_Error_Language_Title,
                    Body = string.Format(Resources.Lang.Resources.LNG_Error_Language_Body, code)
                };
            }
            await ShowDialog.Handle(popup);
        }

        /// <summary>
        /// Creates a About popup filled with translated infomation.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Creates a Folder picker for the progmram to pull EVTC files from and any sub directories.
        /// </summary>
        /// <param name="window">Current window of the program</param>
        /// <returns></returns>
        private async void Folder(Window window)
        {
            TopLevel? topLevel = TopLevel.GetTopLevel(window);
            if (topLevel == null || topLevel.StorageProvider == null)
                return;
            IReadOnlyList<IStorageFolder> responce = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = Resources.Lang.Resources.LNG_Select_Folder,
                SuggestedStartLocation = await topLevel.StorageProvider.TryGetFolderFromPathAsync(_settingService.Path),
                AllowMultiple = false
            });
            if (responce.Any()) 
            {
                _settingService.Path = responce[0].Path.LocalPath;
                _settingService.Save();
                Thread thread = new(() => SearchFolder(false));
                thread.Start();
            }
        }

        /// <summary>
        /// Filters and sorts the currently stored items.
        /// </summary>
        /// <param name="date">Date to filter too</param>
        /// <param name="time">If date is present. What time to filter too</param>
        /// <returns></returns>
        private void Filter(DateTimeOffset? dateFrom = null, TimeSpan? timeFrom = null, DateTimeOffset? dateTo = null, TimeSpan? timeTo = null)
        {
            if (dateFrom.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = dateFrom.Value;
            if (timeFrom.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = _settingService.FilterSettings.TimeOffsetMin.Date + timeFrom.Value;

            if (dateTo.HasValue)
                _settingService.FilterSettings.TimeOffsetMax = dateTo.Value;
            if (timeTo.HasValue)
                _settingService.FilterSettings.TimeOffsetMax = _settingService.FilterSettings.TimeOffsetMax.Date + timeTo.Value;

            _filteredItems = _storedItems.Where(x => _settingService.FilterSettings.Predicate(x)).ToList();
            FileCount = _filteredItems.Count;

            // Sort by creation date
            _filteredItems.Sort((EVTCFile x, EVTCFile y) => y.CreationDate.CompareTo(x.CreationDate));
            Dispatcher.UIThread.Post(() =>
            {
                Items.Clear();
                Items.AddRange(_filteredItems);
                return;
            }, DispatcherPriority.MaxValue);
        }

        /// <summary>
        /// Uploads selected evtc files to dps.report. Requires "DataGrid" element in the window.
        /// </summary>
        /// <param name="window">Current window of the program</param>
        /// <returns></returns>
        private async void UploadAsync(Window window)
        {
            DataGrid? grid = window.Find<DataGrid>("DataGrid");
            if (grid == null)
                return;
            var popup = new PopupViewModel();
            if (grid.SelectedItems.Count > 50 || grid.SelectedItems.Count == 0)
            {
                popup.Title = string.Format(Resources.Lang.Resources.LNG_Error_Upload_Amount, grid.SelectedItems.Count);
                await ShowDialog.Handle(popup);
                return;
            }

            List<EVTCFile> list = grid.SelectedItems.Cast<EVTCFile>().ToList();

            list.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
            ProgressBarMax = list.Count;

            foreach (EVTCFile file in list)
            {
                if (file.UploadUrl == string.Empty)
                {
                    DpsReportResponse? responce = await _uploaderService.UploadEVTCAsync(file.FullPath);
                    if (responce != null && responce.permalink != null)
                        file.UploadUrl = responce.permalink;
                }
                ProgressBarValue++;
            }
            _localDatabaseService.UpdateRecordsURL(list);
            string result = FormatedUploadListString(list);

            TopLevel? topLevel = TopLevel.GetTopLevel(window);
            if (topLevel != null && topLevel.Clipboard != null)
            {
                await topLevel.Clipboard.SetTextAsync(result);
            }

            ProgressBarValue = ProgressBarMax;
            popup.Title = result;
            await ShowDialog.Handle(popup);
            ProgressBarValue = 0;
            
        }

        /// <summary>
        /// Formats the resulting uploaded files urls into single string.
        /// </summary>
        /// <param name="uploadlist">List of files that were uploaded</param>
        /// <returns></returns>
        private static string FormatedUploadListString(List<EVTCFile> uploadlist)
        {
            List<string> clipborad = new()
            {
               string.Format(Resources.Lang.Resources.LNG_Raid_Logs_Title_NewLine, $"{DateTime.Now:D}\n")

            };
            Encounter lastboss = Encounter.Empty;
            foreach (EVTCFile file in uploadlist)
            {
                if (file.UploadUrl == string.Empty)
                    continue;

                if (lastboss == Encounter.Empty || lastboss != file.Boss)
                {
                    clipborad.Add($"{file.Boss}");
                    lastboss = file.Boss;
                }
                clipborad.Add($"{file.UploadUrl}");
            }
            return string.Join("\n", clipborad.ToArray());
        }

        /// <summary>
        /// Clears and recreates the database and removes currently stored files.
        /// </summary>
        /// <returns></returns>
        private void WipeDatabaseAndItems()
        {
            _storedItems.Clear();
            _localDatabaseService.WipeDB();
        }

        /// <summary>
        /// Loads the data from storageIO into the visible item.
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
            SearchFolder(_storedItems.Count != 0);
            Filter();
        }

        /// <summary>
        /// Wipes database and gets files to fill the data base with.
        /// </summary>
        /// <param name="update">Gets files to update the database with</param>
        /// <returns></returns>
        private void SearchFolder(bool update)
        {
            if (!update)
                WipeDatabaseAndItems();
            IEnumerable<string> files = GetFiles(update);
            if (!files.Any())
                return;
            GetItems(files);
        }

        /// <summary>
        /// Gets evtc files from the _settingService.Path and sub directories.
        /// </summary>
        /// <param name="filterAlreadyStored">Filter out files currently stored in _storedItems</param>
        /// <returns></returns>
        private IEnumerable<string> GetFiles(bool filterAlreadyStored)
        {
            IEnumerable<string> files = new List<string>();
            try
            {
                files = Directory.EnumerateFiles(_settingService.Path, "*evtc*", SearchOption.AllDirectories)
                    .Where(s =>
                    s.ToLower().EndsWith(".evtc") ||
                    s.ToLower().EndsWith(".evtc.zip") ||
                    s.ToLower().EndsWith(".zevtc"));
            } 
            catch (Exception e) //TODO:: Add better exception. Popup maybe?
            { 
                Console.WriteLine(e.Message);
            }
            if (filterAlreadyStored)
                return files.Where(s => !_storedItems.Any(val => s.Equals(val.FullPath)));
            return files;
        }

        /// <summary>
        /// Reads list of files and adds them to the database + application.
        /// </summary>
        /// <param name="files">List of file locations to be read</param>
        /// <returns></returns>
        private void GetItems(IEnumerable<string> files)
        {
            List<Task> bagTasks = new();
            List<EVTCFile> EVTCFiles = new();
            try
            {
                ProgressBarMax = files.Count();
                foreach (string file in files)
                {
                    bagTasks.Add(Task.Run(() =>
                    {
                        if (File.Exists(file))
                            EVTCFiles.Add(new(file));
                        ProgressBarValue++;
                    }));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
            Task.WaitAll(bagTasks.ToArray());
            _storedItems.Add(EVTCFiles);
            _localDatabaseService.AddRecords(EVTCFiles);
            Filter();
            ProgressBarValue = 0;
        }
    }
}
