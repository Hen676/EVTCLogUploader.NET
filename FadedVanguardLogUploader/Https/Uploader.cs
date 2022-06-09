using FadedVanguardLogUploader.Models.Responce;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.Https
{
    internal class Uploader
    {
        private static readonly string dpsReportUrl = "https://dps.report/uploadContent?json=1&generator=ei";
        private static readonly HttpClient client = new();

        public static async Task<DpsReportResponse?> UploadEVTCAsync(FileInfo evtc)
        {
            HttpResponseMessage response;
            using (var form = new MultipartFormDataContent())
            {
                using var stream = evtc.OpenRead();
                form.Add(new StreamContent(stream), "file", evtc.ToString());
                response = await client.PostAsync(dpsReportUrl, form);
            }
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DpsReportResponse>(json);
        }
    }
}
