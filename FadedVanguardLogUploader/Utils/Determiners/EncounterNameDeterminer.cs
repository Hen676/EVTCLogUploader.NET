using EVTCLogUploader.Enums;

namespace EVTCLogUploader.Utils.Determiners
{
    public static class EncounterNameDeterminer
    {
        public static string Result(Encounter encounter) => encounter switch
        {
            Encounter.Unkown => "Error, Unkown encounter",
            Encounter.Adina => "Cardinal Adina",
            Encounter.AetherbladeHideout => "Aetherblade Hideout",
            Encounter.AiKeeperOfThePeak => "Sorrowful Spellcaster",
            Encounter.Arkk => "Arkk",
            Encounter.Artsariiv => "Artsariiv",
            Encounter.BanditTrio => "Bandit Trio",
            Encounter.Boneskinner => "Boneskinner",
            Encounter.BrokenKing => "Broken King",
            Encounter.Cairn => "Cairn the Indomitable",
            Encounter.ColdWar => "Cold War",
            Encounter.ConjuredAmalgamate => "Conjured Amalgamate",
            Encounter.Deimos => "Deimos",
            Encounter.Dhuum => "Dhuum",
            Encounter.EaterOfSouls => "Eater Of Souls",
            Encounter.Ensolyss => "Ensolyss",
            Encounter.Escort => "Escort",
            Encounter.Eyes => "Eye of Judgment & Eye of Fate",
            Encounter.ForgingSteel => "Forging Steel",
            Encounter.FraenirOfJormag => "Fraenir Of Jormag",
            Encounter.Freezie => "Freezie",
            Encounter.Golem => "Golem",
            Encounter.Gorseval => "Gorseval the Multifarious",
            Encounter.HarvestTemple => "Harvest Temple",
            Encounter.KainengOverlook => "Kaineng Overlook",
            Encounter.KeepConstruct => "Keep Construct",
            Encounter.MAMA => "MAMA",
            Encounter.Mattias => "Mattias",
            Encounter.MursaatOverseer => "Mursaat Overseer",
            Encounter.Qadim => "Qadim",
            Encounter.QadimThePeerless => "Qadim The Peerless",
            Encounter.RiverOfSouls => "River Of Souls",
            Encounter.Sabetha => "Sabetha the Saboteur",
            Encounter.Sabir => "Cardinal Sabir",
            Encounter.Samarog => "Samarog",
            Encounter.ShiverpeaksPass => "Shiverpeaks Pass",
            Encounter.Siax => "Siax the Unclean",
            Encounter.Skorvald => "Skorvald the Shattered",
            Encounter.Slothasor => "Slothasor",
            Encounter.SoullessHorror => "Soulless Horror",
            Encounter.TwinLargos => "Twin Largos",
            Encounter.TwistedCastle => "Twisted Castle",
            Encounter.ValeGuardian => "Vale Guardian",
            Encounter.VoiceAndClawOfTheFallen => "Voice And Claw Of The Fallen",
            Encounter.WhisperOfJormag => "Whisper Of Jormag",
            Encounter.Xera => "Xera",
            Encounter.XunlaiJadeJunkyard => "Xunlai Jade Junkyard",
            _ => "Error, Unssuported encounter",
        };
    }
}
