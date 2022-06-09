namespace FadedVanguardLogUploader.Models.Responce
{
    public class EncounterResponce
    {
        public string? uniqueId { get; set; }
        public bool success { get; set; }
        public long duration { get; set; }
        public int compDps { get; set; }
        public int numberOfPlaye { get; set; }
        public int numberOfGroup { get; set; }
        public int bossId { get; set; }
        public string? boss { get; set; }
        public bool isCm { get; set; }
        public int gw2Build { get; set; }
        public bool jsonAvailable { get; set; }
    }
}
