using EVTCLogUploader.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    public interface ILocalDatabaseService
    {
        void AddRecords(List<EVTCFile> records);
        Task<List<EVTCFile>> GetRecords();
        void UpdateRecordsURL(List<EVTCFile> newValues);
        void WipeDB();
    }
}
