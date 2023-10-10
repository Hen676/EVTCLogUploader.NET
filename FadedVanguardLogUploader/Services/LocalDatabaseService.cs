using EVTCLogUploader.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    public class LocalDatabaseService : ILocalDatabaseService
    {
        private SQLiteAsyncConnection Database;
        private const SQLiteOpenFlags Flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;
        public string DatabasePath;
        bool initalised = false;

        public LocalDatabaseService() 
        {
            DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "data.db");
            Database = new(DatabasePath, Flags);
        }

        public LocalDatabaseService(string path)
        {
            DatabasePath = path;
            Database = new(path, Flags);
        }

        private async void Init()
        {
            await Database.CreateTableAsync<EVTCFile>();
            initalised = true;
        }

        public async void AddRecords(List<EVTCFile> records)
        {
            if (!initalised)
                Init();
            await Database.InsertAllAsync(records);
        }

        public async Task<List<EVTCFile>> GetRecords()
        {
            if (!initalised)
                Init();
            return await Database.Table<EVTCFile>().ToListAsync();
        }

        public async void UpdateRecordsURL(List<EVTCFile> newValues)
        {
            if (!initalised)
                Init();
            await Database.UpdateAllAsync(newValues);
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
