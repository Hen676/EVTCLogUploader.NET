using System.IO;

namespace FadedVanguardLogUploader.Models
{
    public class ListEVTC
    {
        public FileInfo FileInfo { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
        private static int i = 0;

        public ListEVTC(string path)
        {
            FileInfo = new FileInfo(path);
            IsSelected = false;
            Name = FileInfo.Name + " " + i;
            i++;
        }
    }
}
