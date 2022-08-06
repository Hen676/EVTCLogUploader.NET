using FadedVanguardLogUploader.Enums;
using System;
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

        [DefaultSettingValue(nameof(SortingType.DateDescending)), UserScopedSetting]
        public SortingType SortingType
        {
            get => (SortingType)this[nameof(SortingType)];
            set
            {
                this[nameof(SortingType)] = value;
            }
        }

        [DefaultSettingValue("25"), UserScopedSetting]
        public int PageAmount
        {
            get => (int)this[nameof(PageAmount)];
            set
            {
                this[nameof(PageAmount)] = value;
            }
        }
    }
}
