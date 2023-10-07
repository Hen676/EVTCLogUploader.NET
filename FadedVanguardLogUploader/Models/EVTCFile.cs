using EVTCLogUploader.Enums;
using EVTCLogUploader.Services.IO;
using SQLite;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace EVTCLogUploader.Models
{
    public class EVTCFile
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.MinValue;
        public string MainUserName { get; set; } = string.Empty;
        public string MainCharcterName { get; set; } = string.Empty;
        public TimeSpan Length { get; set; } = TimeSpan.Zero;
        public Profession CharcterClassOfMainUser { get; set; } = Profession.Unknown;
        public Specialization CharcterSpecOfMainUser { get; set; } = Specialization.None;
        public Encounter Boss { get; set; } = Encounter.Unkown;
        public string UploadUrl { get; set; } = string.Empty;
        public bool Success { get; set; } = false; // TODO:: Remove?
        public FileType Type { get; set; } = FileType.None;

        [Ignore]
        public string ProfAndSpec { get; set; } = string.Empty;


#if DEBUG
        private static int i = 0;
#endif
        #region Constructor
        public EVTCFile() { }

        public EVTCFile(string fullPath, string name, DateTime creationDate, string userName, string charcterName, TimeSpan length, Profession charcterClass, Specialization charcterSpec, Encounter encounter, string uploadUrl, FileType fileType)
        {
            FullPath = fullPath;
            Name = name;
            CreationDate = creationDate;
            MainUserName = userName;
            MainCharcterName = charcterName;
            Length = length;
            CharcterClassOfMainUser = charcterClass;
            CharcterSpecOfMainUser = charcterSpec;
            Boss = encounter;
            UploadUrl = uploadUrl;
            Type = fileType;
            LoadDisplayInfomation();
        }

        public EVTCFile(string path)
        {
            FullPath = path;
            FileInfo fileInfo = new(FullPath);
            Name = fileInfo.Name;
            CreationDate = fileInfo.CreationTime;
            LoadData();
            LoadDisplayInfomation();
#if DEBUG
            Name = i + " " + Name;
            i++;
#endif
        }
        #endregion

        public void LoadData()
        {
            BinaryArrayReaderIO reader;
            bool isZEVTC = FullPath.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase);

            // Uncompress EVTC if needed
            if (FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                || isZEVTC)
            {
                Type = isZEVTC ? FileType.ZEVTC : FileType.EVTCZIP;

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
                Type = FileType.EVTC;
                reader = new BinaryArrayReaderIO(File.ReadAllBytes(FullPath), new UTF8Encoding());
            }

            // Read Log
            BinaryReaderHandlerIO handler = new(reader);
            Boss = handler.GetEncounter();
            Length = handler.GetLength();
            MainCharcterName = handler.GetCharcterName();
            MainUserName = handler.GetUserName();
            CharcterClassOfMainUser = handler.GetCharcterProf();
            CharcterSpecOfMainUser = handler.GetCharcterSpec();
            CreationDate = handler.GetServerDateTime();
        }

        public void LoadDisplayInfomation() 
        {
            if (CharcterSpecOfMainUser == Specialization.None)
            {
                if (CharcterClassOfMainUser == Profession.Unknown)
                    ProfAndSpec = "Error, Unkown profession";
                ProfAndSpec = CharcterClassOfMainUser.ToString();
            }
            else
            {
                if (CharcterSpecOfMainUser == Specialization.Empty)
                    ProfAndSpec = "Error, Unkown specialization";
                ProfAndSpec = CharcterSpecOfMainUser.ToString();
            }
        }
    }
}
