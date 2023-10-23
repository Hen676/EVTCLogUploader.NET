using EVTCLogUploader.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    public class LocalDatabaseService : ILocalDatabaseService
    {
        public string DatabasePath;
        private readonly SQLiteAsyncConnection _database;
        private const SQLiteOpenFlags _flags =
        SQLiteOpenFlags.ReadWrite |
        SQLiteOpenFlags.Create |
        SQLiteOpenFlags.SharedCache;
        private bool _initalised = false;

        public LocalDatabaseService()
        {
            DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "data.db");
            _database = new(DatabasePath, _flags);
        }

        public LocalDatabaseService(string path)
        {
            DatabasePath = path;
            _database = new(path, _flags);
        }

        private async void Init()
        {
            await _database.CreateTableAsync<EVTCFile>();
            _initalised = true;
        }

        public async void AddRecords(List<EVTCFile> records)
        {
            if (!_initalised)
                Init();
            await _database.InsertAllAsync(records);
        }

        public async Task<List<EVTCFile>> GetRecords()
        {
            if (!_initalised)
                Init();
            return await _database.Table<EVTCFile>().ToListAsync();
        }

        public async void UpdateRecordsURL(List<EVTCFile> newValues)
        {
            if (!_initalised)
                Init();
            await _database.UpdateAllAsync(newValues);
        }

        public async void WipeDB()
        {
            if (!_initalised)
                return;
            await _database.DropTableAsync<EVTCFile>();
            Init();
        }
    }
}
