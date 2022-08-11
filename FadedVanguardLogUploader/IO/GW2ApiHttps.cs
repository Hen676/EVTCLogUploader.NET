using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models.Responce;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace FadedVanguardLogUploader.IO
{
    internal class GW2ApiHttps
    {
        private static readonly Uri gw2ApiBaseUrl = new("https://api.guildwars2.com/v2/");
        private static readonly HttpClient client = new();

        public static Dictionary<Profession, string> ProfIcons = new();
        public static Dictionary<Specialization, string> SpecIcons = new();

        public static void Init()
        {
            GetProfessionIcons();
            GetSpecializationIcons();
        }

        private static async void GetProfessionIcons()
        {
            ProfIcons.Clear();
            foreach (Profession prof in (Profession[])Enum.GetValues(typeof(Profession)))
            {
                if (prof == Profession.Unknown)
                    continue;

                HttpResponseMessage response = await client.GetAsync(gw2ApiBaseUrl + "professions/" + nameof(prof));
                if (!response.IsSuccessStatusCode)
                    return;

                string json = await response.Content.ReadAsStringAsync();
                ProfessionResponce? professionResponce = JsonConvert.DeserializeObject<ProfessionResponce>(json);
                if (professionResponce == null)
                    return;

                ProfIcons.Add(prof, professionResponce.icon);
            }
        }

        private static async void GetSpecializationIcons()
        {
            foreach (Specialization spec in (Specialization[])Enum.GetValues(typeof(Specialization)))
            {
                if (spec == Specialization.None || spec == Specialization.Empty)
                    continue;

                HttpResponseMessage response = await client.GetAsync(gw2ApiBaseUrl + "specializations/" + ((int)spec));
                if (!response.IsSuccessStatusCode)
                    return;

                var json = await response.Content.ReadAsStringAsync();
                SpecializationIconResponce? SpecializationResponce = JsonConvert.DeserializeObject<SpecializationIconResponce>(json);
                if (SpecializationResponce == null)
                    return;

                SpecIcons.Add(spec, SpecializationResponce.profession_icon);
            }
        }
    }
}
