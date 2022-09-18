using FadedVanguardLogUploader.Enums;
using System.Collections.Generic;
using System.Configuration;

namespace FadedVanguardLogUploader.Settings
{
    public class AppSettings : ApplicationSettingsBase
    {
        [DefaultSettingValue(""), UserScopedSetting]
        public string Path
        {
            get => (string)this[nameof(Path)];
            set
            {
                this[nameof(Path)] = value;
            }
        }

        [DefaultSettingValue("false"), UserScopedSetting]
        public bool ModeToggle
        {
            get => (bool)this[nameof(ModeToggle)];
            set
            {
                this[nameof(ModeToggle)] = value;
            }
        }

        [DefaultSettingValue("false"), UserScopedSetting]
        public bool ApiToggle
        {
            get => (bool)this[nameof(ApiToggle)];
            set
            {
                this[nameof(ApiToggle)] = value;
            }
        }

        [DefaultSettingValue(nameof(SortingType.Date)), UserScopedSetting]
        public SortingType SortingType
        {
            get => (SortingType)this[nameof(SortingType)];
            set
            {
                this[nameof(SortingType)] = value;
            }
        }

        [DefaultSettingValue("true"), UserScopedSetting]
        public bool ErrorFilterToggle
        {
            get => (bool)this[nameof(ErrorFilterToggle)];
            set
            {
                this[nameof(ErrorFilterToggle)] = value;
            }
        }

        [DefaultSettingValue("true"), UserScopedSetting]
        public bool SortingToggle
        {
            get => (bool)this[nameof(SortingToggle)];
            set
            {
                this[nameof(SortingToggle)] = value;
            }
        }

        [DefaultSettingValue("en-GB"), UserScopedSetting]
        public string Lang 
        {
            get => (string)this[nameof(Lang)];
            set
            {
                this[nameof(Lang)] = value;
            }
        }

        [DefaultSettingValue(""), UserScopedSetting]
        public List<Encounter> FilterEncounter
        {
            get => (List<Encounter>)this[nameof(FilterEncounter)];
            set
            {
                this[nameof(FilterEncounter)] = value;
            }
        }
    }
}
