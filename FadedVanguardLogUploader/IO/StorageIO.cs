﻿using CsvHelper;
using CsvHelper.Configuration;
using FadedVanguardLogUploader.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FadedVanguardLogUploader.IO
{
    public class StorageIO
    {
        private readonly static string storageName = "\\data.csv";
        private readonly string storagePath;
        private readonly CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = true
        };
        private readonly CsvConfiguration updateConfiguration = new(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
            HasHeaderRecord = false
        };

        public StorageIO()
        {
            storagePath = Directory.GetCurrentDirectory() + storageName;
        }

        public void OpenCSV() 
        {
            if (File.Exists(storagePath))
                Process.Start(new ProcessStartInfo(storagePath) { UseShellExecute = true });
        }

        public List<ListItem> Get()
        {
            if (!File.Exists(storagePath))
                return new();
            using var streamReader = new StreamReader(storagePath);
            using var csv = new CsvReader(streamReader, configuration);
            if (csv == null)
                return new();
            var x = csv.GetRecords<ListItem>().ToList();
            streamReader.Close();
            return x;
        }

        public void Create(List<ListItem> records)
        {
            var streamWriter = new StreamWriter(storagePath);
            using (streamWriter)
            {
                using var csv = new CsvWriter(streamWriter, configuration);
                csv.WriteRecords(records);
            }
            streamWriter.Close();
        }

        public void Update(List<ListItem> newValues)
        {
            var stream = File.Open(storagePath, FileMode.Append);
            using (stream)
            {
                using var streamWriter = new StreamWriter(stream);
                using var csv = new CsvWriter(streamWriter, updateConfiguration);
                csv.WriteRecords(newValues);
            }
            stream.Close();
        }

        public void Delete()
        {
            File.Delete(storagePath);
        }
    }
}
