using EVTCLogUploader.Utils;

namespace EVTCLogUploader.Services
{
    public interface ISettingService
    {
        string Path { get; set; }
        Filter FilterSettings { get; set; }
        string Language { get; set; }
        bool ModeToggle { get; set; }

        public void Save();
        bool SetDirectory(string dir);
        bool SetLanguage(string code);
    }
}
