using CsvHelper;
using CsvHelper.Configuration;
using FadedVanguardLogUploader.Models;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace FadedVanguardLogUploader.IO
{
    public class StorageIO
    {
        private readonly static string storageName = "\\data.csv";
        private readonly string storagePath;
        private readonly CsvConfiguration configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ","
        };

        public StorageIO()
        {
            storagePath = Directory.GetCurrentDirectory() + storageName;
        }

        public List<ListItem> Get()
        {
            if (!File.Exists(storagePath))
                return new();
            using var streamReader = new StreamReader(storagePath);
            using var csv = new CsvReader(streamReader, configuration);
            return csv.GetRecords<ListItem>().ToList();
        }

        public void Create(List<ListItem> records)
        {
            using var streamWriter = new StreamWriter(storagePath);
            using var csv = new CsvWriter(streamWriter, configuration);
            csv.WriteRecords(records);
        }

        public void Update(List<ListItem> newValues)
        {
            using var stream = File.Open(storagePath, FileMode.Append);
            using var streamWriter = new StreamWriter(stream);
            using var csv = new CsvWriter(streamWriter, configuration);
            csv.WriteRecords(newValues);
        }

        public void Delete()
        {
            File.Delete(storagePath);
        }
    }
}
