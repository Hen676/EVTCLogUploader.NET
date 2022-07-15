using CsvHelper.Configuration.Attributes;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.IO;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FadedVanguardLogUploader.Models
{
    public class ListItem
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.MinValue;
        public string UserName { get; set; } = string.Empty;
        public string CharcterName { get; set; } = string.Empty;
        public TimeSpan Length { get; set; } = TimeSpan.Zero;
        public Profession CharcterClass { get; set; } = Profession.Unknown;
        public Specialization CharcterSpec { get; set; } = Specialization.None;
        public Encounter Encounter { get; set; } = Encounter.Unkown;


        [Ignore]
        public bool IsSelected { get; set; }

#if DEBUG
        [Ignore]
        private static int i = 0;
#endif

        public ListItem()
        {
            IsSelected = false;
        }

        public ListItem(string path)
        {
            FullPath = path;
            FileInfo fileInfo = new(FullPath);
            Name = fileInfo.Name;
            CreationDate = fileInfo.CreationTime;
            IsSelected = false;
            LoadData();
#if DEBUG
            Name += i;
            i++;
#endif
        }

        public void LoadData()
        {
            BinaryArrayReaderIO reader;

            // Uncompress EVTC if needed
            if (FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                || FullPath.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
            {
                using ZipArchive zip = ZipFile.Open(FullPath, ZipArchiveMode.Read);
                using Stream data = zip.Entries[0].Open();
                var bytes = new byte[zip.Entries[0].Length];
                int read;
                int offset = 0;
                while ((read = data.Read(bytes, offset, bytes.Length - offset)) > 0)
                {
                    offset += read;
                }

                if (bytes.Length == 0)
                    return;
                reader = new BinaryArrayReaderIO(bytes, new UTF8Encoding());
            }
            else
                reader = new BinaryArrayReaderIO(File.ReadAllBytes(FullPath), new UTF8Encoding());

            // Read Log
            BinaryReaderHandlerIO handler = new(reader);
            Encounter = handler.GetEncounter();
            Length = handler.GetLength();
            CharcterName = handler.GetCharcterName();
            UserName = handler.GetUserName();
            CharcterClass = handler.GetCharcterProf();
            CharcterSpec = handler.GetCharcterSpec();
            CreationDate = handler.GetServerDateTime();
            Length = handler.GetLength();
        }
    }
}
