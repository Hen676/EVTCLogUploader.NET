using EVTCLogUploader.Enums;
using EVTCLogUploader.IO;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace EVTCLogUploader.Models
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
        public string UploadUrl { get; set; } = string.Empty;
        public bool Success { get; set; } = false;

        public string ProfAndSpec { get; set; } = string.Empty;
        public string ProfAndSpecIcon { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;
        public FileType FileType { get; set; } = FileType.None;

#if DEBUG
        private static int i = 0;
#endif

        public ListItem(string FullPath, string Name, DateTime CreationDate, string UserName, string CharcterName, TimeSpan Length, Profession CharcterClass, Specialization CharcterSpec, Encounter Encounter, string UploadUrl, FileType FileType)
        {
            this.FullPath = FullPath;
            this.Name = Name;
            this.CreationDate = CreationDate;
            this.UserName = UserName;
            this.CharcterName = CharcterName;
            this.Length = Length;
            this.CharcterClass = CharcterClass;
            this.CharcterSpec = CharcterSpec;
            this.Encounter = Encounter;
            this.UploadUrl = UploadUrl;
            this.FileType = FileType;
            LoadDisplayInfomation(CharcterClass, CharcterSpec);
        }

        public ListItem(string path)
        {
            FullPath = path;
            FileInfo fileInfo = new(FullPath);
            Name = fileInfo.Name;
            CreationDate = fileInfo.CreationTime;
            LoadData();
            LoadDisplayInfomation(CharcterClass, CharcterSpec);
#if DEBUG
            Name += i;
            i++;
#endif
        }

        public void LoadData()
        {
            BinaryArrayReaderIO reader;
            bool isZEVTC = FullPath.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase);

            // Uncompress EVTC if needed
            if (FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                || isZEVTC)
            {
                FileType = isZEVTC ? FileType.ZEVTC : FileType.EVTCZIP;

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
            {
                FileType = FileType.EVTC;
                reader = new BinaryArrayReaderIO(File.ReadAllBytes(FullPath), new UTF8Encoding());
            }

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

        public void LoadDisplayInfomation(Profession prof, Specialization spec) 
        {
            if (spec == Specialization.None)
            {
                if (prof == Profession.Unknown)
                    ProfAndSpec = "Error, Unkown profession";
                ProfAndSpec = prof.ToString();
            }
            else
            {
                if (spec == Specialization.Empty)
                    ProfAndSpec = "Error, Unkown specialization";
                ProfAndSpec = spec.ToString();
            }
        }
    }
}
