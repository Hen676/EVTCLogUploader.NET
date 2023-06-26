using Avalonia.Controls;
using DynamicData;
using EVTCLogUploader.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVTCLogUploader.Settings
{
    public class SettingsService
    {
        public SettingsService()
        { }

        public void save() => App.Settings.Save();

        public void ClearFilters()
        {
            App.Settings.FilterEncounter.Clear();
        }

        public void setLangaugeCode(string code)
        {
            App.Settings.Lang = code;
        }

        public SortingType setSortingType(SortingType type)
        {
            App.Settings.SortingType = type;
            return App.Settings.SortingType;
        }

        public bool AscDesToggle()
        {
            App.Settings.SortingToggle = !App.Settings.SortingToggle;
            return App.Settings.SortingToggle;
        }

        public bool ModeToggle()
        {
            App.Settings.ModeToggle = !App.Settings.ModeToggle;
            return App.Settings.ModeToggle;
        }

        public bool ErrorFilterToggle()
        {
            App.Settings.ErrorFilterToggle = !App.Settings.ErrorFilterToggle;
            return App.Settings.ModeToggle;
        }

        internal ObservableCollection<Encounter> EditEncounterList(Encounter encounter)
        {
            if (!App.Settings.FilterEncounter.Contains(encounter))
                App.Settings.FilterEncounter.Add(encounter);
            else
                App.Settings.FilterEncounter.Remove(encounter);
            return new(App.Settings.FilterEncounter);
        }

        internal ObservableCollection<FileType> EditFileTypeList(FileType fileType)
        {
            if (!App.Settings.FilterFileType.Contains(fileType))
                App.Settings.FilterFileType.Add(fileType);
            else
                App.Settings.FilterFileType.Remove(fileType);
            return new(App.Settings.FilterFileType);
        }

        internal ObservableCollection<Profession> EditProfessionList(Profession profession)
        {
            if (!App.Settings.FilterProfession.Contains(profession))
                App.Settings.FilterProfession.Add(profession);
            else
                App.Settings.FilterProfession.Remove(profession);
            return new(App.Settings.FilterProfession);
        }
    }
}
