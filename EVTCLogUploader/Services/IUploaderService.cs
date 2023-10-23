using EVTCLogUploader.Models.Responce;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    public interface IUploaderService
    {
        Task<DpsReportResponse?> UploadEVTCAsync(string evtcPath);
    }
}
