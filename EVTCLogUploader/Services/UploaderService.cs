﻿using EVTCLogUploader.Models.Responce;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace EVTCLogUploader.Services
{
    internal class UploaderService : IUploaderService
    {
        private static readonly string dpsReportUrl = "https://dps.report/uploadContent?json=1&generator=ei";
        private readonly HttpClient client = new();

        public async Task<DpsReportResponse?> UploadEVTCAsync(string evtcPath)
        {
            HttpResponseMessage response;
            using (var form = new MultipartFormDataContent())
            {
                FileInfo fileInfo = new(evtcPath);
                using var stream = fileInfo.OpenRead();
                form.Add(new StreamContent(stream), "file", fileInfo.ToString());
                response = await client.PostAsync(dpsReportUrl, form);
            }
            if (!response.IsSuccessStatusCode)
                return null;

            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DpsReportResponse>(json);
        }
    }
}
