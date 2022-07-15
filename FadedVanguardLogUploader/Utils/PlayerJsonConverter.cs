using FadedVanguardLogUploader.Models.Responce;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace FadedVanguardLogUploader.Util
{
    public class PlayerJsonConverter : JsonConverter<List<PlayerResponce>>
    {
        public override void WriteJson(JsonWriter writer, List<PlayerResponce>? value, JsonSerializer serializer)
        {
            if (value == null)
                return;

            writer.WriteStartArray();
            foreach (PlayerResponce player in value)
            {
                writer.WriteRawValue(JsonConvert.SerializeObject(player));
            }
            writer.WriteEndArray();
        }

        public override List<PlayerResponce>? ReadJson(JsonReader reader, Type objectType, List<PlayerResponce>? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var response = new List<PlayerResponce>();
            JObject players = JObject.Load(reader);
            foreach (var player in players)
            {
                if (player.Value == null)
                    continue;
                var p = JsonConvert.DeserializeObject<PlayerResponce>(player.Value.ToString());
                if (p == null)
                    continue;
                response.Add(p);
            }

            return response;
        }
    }
}
