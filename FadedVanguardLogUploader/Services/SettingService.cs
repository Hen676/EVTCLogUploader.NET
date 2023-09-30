using EVTCLogUploader.Enums;
using EVTCLogUploader.Utils;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace EVTCLogUploader.Services
{
    internal class SettingService : ApplicationSettingsBase, ISettingService
    {
        /** Settings service
         *  
         *  Stores all settings for the application
         *  
         *  TODO:: Save on application close
         */

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
        public bool SortingToggle
        {
            get => (bool)this[nameof(SortingToggle)];
            set
            {
                this[nameof(SortingToggle)] = value;
            }
        }

        [DefaultSettingValue("en-GB"), UserScopedSetting]
        public string Language
        {
            get => (string)this[nameof(Language)];
            set
            {
                this[nameof(Language)] = value;
            }
        }

        [DefaultSettingValue(""), UserScopedSetting]
        public Filter FilterSettings
        {
            get => (Filter)this[nameof(FilterSettings)];
            set
            {
                this[nameof(FilterSettings)] = value;
            }
        }

        private static readonly List<string> supportedLanguages = new List<string>
        {
            "en",
            "fr",
            "de",
            "it",
            "pl-PL",
            "pt_BR"
        };

        public bool SetLanguage(string code)
        {
            if (!supportedLanguages.Contains(code))
                return false;
            Language = code;
            return true;
        }

        public bool SetDirectory(string dir)
        {
            if (!Directory.Exists(dir))
                return false;
            Path = dir;
            return true;
        }
    }
}
