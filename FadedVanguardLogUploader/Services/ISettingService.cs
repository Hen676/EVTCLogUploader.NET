using EVTCLogUploader.Enums;
using EVTCLogUploader.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    public interface ISettingService
    {
        string Path { get; set; }
        Filter FilterSettings { get; set; }
        string Language { get; set; }
        bool SortingToggle { get; set; }
        SortingType SortingType { get; set; }
        bool ModeToggle { get; set; }

        public void Save();
        bool SetDirectory(string dir);
        bool SetLanguage(string code);
    }
}
