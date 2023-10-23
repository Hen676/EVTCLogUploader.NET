using EVTCLogUploader.Utils.Converter;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EVTCLogUploader.Models.Responce
{
    public class DpsReportResponse
    {
        // Start
        public string? id { get; set; }
        public string? permalink { get; set; }
        public int uploadTime { get; set; }
        public int encounterTime { get; set; }
        public string? generator { get; set; }
        public int generatorId { get; set; }
        public int generatorVersion { get; set; }
        public string? language { get; set; }
        public int languageId { get; set; }

        // Class responces
        public EvtcResponce? evtc { get; set; }
        [JsonConverter(typeof(PlayerJsonConverter))]
        public List<PlayerResponce>? players { get; set; }
        public EncounterResponce? encounter { get; set; }

        // End
        public string? userToken { get; set; }
        public string? error { get; set; }
    }
}