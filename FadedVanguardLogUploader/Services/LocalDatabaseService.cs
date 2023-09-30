using EVTCLogUploader.Models.EVTCList;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    internal class LocalDatabaseService : ILocalDatabaseService
    {
        private SQLiteAsyncConnection Database = new SQLiteAsyncConnection(DatabasePath, Flags);
        private const string DatabaseFilename = "data.db";
        private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;
        public static string DatabasePath =>
        Path.Combine(Directory.GetCurrentDirectory(), DatabaseFilename);
        bool initalised = false;

        private async void Init()
        {
            await Database.CreateTableAsync<EVTCFile>();
            initalised = true;
        }

        public void AddRecords(List<EVTCFile> records)
        {
            if (!initalised)
                Init();
            Database.UpdateAllAsync(records);
        }

        public async Task<List<EVTCFile>> GetRecords()
        {
            if (!initalised)
                Init();
            return await Database.Table<EVTCFile>().ToListAsync();
        }

        public void UpdateRecordsURL(List<EVTCFile> newValues)
        {
            if (!initalised)
                Init();
            Database.UpdateAllAsync(newValues);
        }

        public async void WipeDB()
        {
            if (!initalised)
                return;
            await Database.DropTableAsync<EVTCFile>();
            Init();
        }
    }
}
