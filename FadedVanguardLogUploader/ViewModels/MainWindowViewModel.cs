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
using DynamicData.Kernel;
using DynamicData;
using Avalonia;
using Avalonia.Styling;
using EVTCLogUploader.Utils.Determiners;
using EVTCLogUploader.Models;
using System.Collections.Specialized;

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
        public ReactiveCommand<Window, Unit> CloseCommand { get; }
        public ReactiveCommand<Window, Unit> FolderCommand { get; }
        public ReactiveCommand<Unit, Unit> UploadCommand { get; }
        #endregion

        public ObservableCollection<EVTCFile> Items { get; } = new();
        public ObservableCollection<EVTCFile> SelectedItems { get; } = new();
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
                new EncounterNode(Resources.Lang.Resources.LNG_Menu_Encounter_SunquaPeak, new ObservableCollection<EncounterNode>{
                    new EncounterNode(Encounter.AiKeeperOfThePeak)
                })
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

        #region Private
        private List<EVTCFile> _storedItems = new();
        private List<EVTCFile> _filteredItems = new();
        private int _fileCount = 0;
        private int _progressBarValue = 0;
        private int _progressBarMax = 100;
        private ThemeVariant _theme;
        private bool _filterError;

        #region Serivces
        private IUploaderService _uploaderService;
        private ISettingService _settingService;
        private ILocalDatabaseService _localDatabaseService;
        #endregion

        #endregion

        #region Constructors
        public MainWindowViewModel()
        {
            // Only use in debug check
            _uploaderService = new UploaderService();
            _settingService = new SettingService();
            _localDatabaseService = new LocalDatabaseService();

            _theme = ThemeVarientDeterminer.Result(_settingService.ModeToggle);
            _filterError = _settingService.FilterSettings.ErrorFilter;

            SelectedFilterNodes.CollectionChanged += SelectedFilterNodes_CollectionChanged;

            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(ThemeVarient);
            LanguageCommand = ReactiveCommand.Create<string>(ChangeLanguageAsync);

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

            _theme = ThemeVarientDeterminer.Result(_settingService.ModeToggle);
            _filterError = _settingService.FilterSettings.ErrorFilter;

            SelectedFilterNodes.CollectionChanged += SelectedFilterNodes_CollectionChanged;

            AboutCommand = ReactiveCommand.Create(About);
            SaveCommand = ReactiveCommand.Create(Save);
            ModeCommand = ReactiveCommand.Create(ThemeVarient);
            LanguageCommand = ReactiveCommand.Create<string>(ChangeLanguageAsync);

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

        private void SelectedFilterNodes_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ObservableCollection<EncounterNode> list && list != null) {
                _settingService.FilterSettings.EditEncounterList(list.ToList());
                Filter();
            }
        }

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
            Filter();
        }
        private void ErrorHidden()
        {
            _settingService.FilterSettings.ErrorFilter = !_settingService.FilterSettings.ErrorFilter;
            FilterError = _settingService.FilterSettings.ErrorFilter;
            Filter();
        }
        #endregion

        private void Close(Window window) => window.Close();
        public void Save() => _settingService.Save();
        private void WipeDB() => _localDatabaseService.WipeDB();

        private void ThemeVarient()
        {
            _settingService.ModeToggle = !_settingService.ModeToggle;
            Theme = ThemeVarientDeterminer.Result(_settingService.ModeToggle);
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
        /// Filters and sorts the current stored items.
        /// </summary>
        /// <param name="date">Date to filter too</param>
        /// <param name="time">If date is present. What time to filter too</param>
        /// <param name="select">Should the items be selected</param>
        /// <returns></returns>
        private void Filter(DateTimeOffset? date = null, TimeSpan? time = null)
        {
            if (date.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = date.Value;
            if (time.HasValue)
                _settingService.FilterSettings.TimeOffsetMin = _settingService.FilterSettings.TimeOffsetMin.Date + time.Value;

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
            if (SelectedItems.Count > 50 || SelectedItems.Count == 0)
            {
                popup.Title = "Error: Inavlid amount of files to upload " + SelectedItems.Count + "/50";
                await ShowDialog.Handle(popup);
                return;
            }
            SelectedItems.ToList().Sort((x, y) => x.CreationDate.CompareTo(y.CreationDate));
            ProgressBarMax = SelectedItems.Count;

            foreach (EVTCFile file in SelectedItems)
            {
                if (file.UploadUrl == string.Empty)
                {
                    DpsReportResponse? responce = await _uploaderService.UploadEVTCAsync(file.FullPath);
                    if (responce != null && responce.permalink != null)
                        file.UploadUrl = responce.permalink;
                }
                ProgressBarValue++;
            }
            _localDatabaseService.UpdateRecordsURL(SelectedItems.ToList());
            string result = FormatedUploadListString(SelectedItems.ToList());
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

                if (lastboss == Encounter.Empty || lastboss != file.Boss)
                {
                    clipborad.Add($"{file.Boss}");
                    lastboss = file.Boss;
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
            IEnumerable<string> files = Directory.EnumerateFiles(_settingService.Path, "*evtc*", SearchOption.AllDirectories)
                .Where(s =>
                s.ToLower().EndsWith(".evtc") ||
                s.ToLower().EndsWith(".evtc.zip") ||
                s.ToLower().EndsWith(".zevtc"));

            if (filterAlreadyStored)
                return files.Where(s => !_storedItems.Any(val => s.Equals(val.FullPath)));
            return files;
            /*
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
                s.ToLower().EndsWith(".zevtc"));*/
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
