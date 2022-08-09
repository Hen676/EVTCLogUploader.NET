using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models.Responce;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FadedVanguardLogUploader.IO
{
    internal class GW2ApiHttps
    {
        private static readonly Uri gw2ApiBaseUrl = new("https://api.guildwars2.com/v2/");
        private static readonly HttpClient client = new();

        public static async Task<Dictionary<Profession, string>?> GetProfessionIcons()
        {
            Dictionary<Profession, string> keyValues = new();
            foreach (Profession prof in (Profession[])Enum.GetValues(typeof(Profession)))
            {
                HttpResponseMessage response = await client.GetAsync(gw2ApiBaseUrl + "professions/" + nameof(prof));
                if (!response.IsSuccessStatusCode)
                    return null;

                string json = await response.Content.ReadAsStringAsync();
                ProfessionResponce? professionResponce = JsonConvert.DeserializeObject<ProfessionResponce>(json);
                if (professionResponce == null)
                    return null;

                keyValues.Add(prof, professionResponce.icon);
            }
            return keyValues;
        }

        public static async Task<Dictionary<Specialization, string>?> GetSpecializationIcons()
        {
            Dictionary<Specialization, string> keyValues = new();
            foreach (Specialization spec in (Specialization[])Enum.GetValues(typeof(Specialization)))
            {
                if (spec == Specialization.None || spec == Specialization.Empty)
                    continue;

                HttpResponseMessage response = await client.GetAsync(gw2ApiBaseUrl + "specializations/" + ((int)spec));
                if (!response.IsSuccessStatusCode)
                    return null;

                var json = await response.Content.ReadAsStringAsync();
                SpecializationIconResponce? SpecializationResponce = JsonConvert.DeserializeObject<SpecializationIconResponce>(json);
                if (SpecializationResponce == null)
                    continue;

                keyValues.Add(spec, SpecializationResponce.profession_icon);
            }
            return keyValues;
        }
    }
}
