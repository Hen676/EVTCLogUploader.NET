using EVTCLogUploader.Enums;

namespace EVTCLogUploader.Utils.Determiners
{
    public static class EncounterNameDeterminer
    {
        public static string Result(Encounter encounter) => encounter switch
        {
            Encounter.Unkown => Resources.Lang.Resources.LNG_Menu_Encounter_Unkown,
            Encounter.Adina => Resources.Lang.Resources.LNG_Menu_Encounter_Adina,
            Encounter.AetherbladeHideout => Resources.Lang.Resources.LNG_Menu_Encounter_AetherbladeHideout,
            Encounter.AiKeeperOfThePeak => Resources.Lang.Resources.LNG_Menu_Encounter_SunquaPeak,
            Encounter.Arkk => Resources.Lang.Resources.LNG_Menu_Encounter_Arkk,
            Encounter.Artsariiv => Resources.Lang.Resources.LNG_Menu_Encounter_Artsariiv,
            Encounter.BanditTrio => Resources.Lang.Resources.LNG_Menu_Encounter_BanditTrio,
            Encounter.Boneskinner => Resources.Lang.Resources.LNG_Menu_Encounter_Boneskinner,
            Encounter.BrokenKing => Resources.Lang.Resources.LNG_Menu_Encounter_BrokenKing,
            Encounter.Cairn => Resources.Lang.Resources.LNG_Menu_Encounter_Cairn,
            Encounter.ColdWar => Resources.Lang.Resources.LNG_Menu_Encounter_ColdWar,
            Encounter.ConjuredAmalgamate => Resources.Lang.Resources.LNG_Menu_Encounter_ConjuredAmalgamate,
            Encounter.Deimos => Resources.Lang.Resources.LNG_Menu_Encounter_Deimos,
            Encounter.Dhuum => Resources.Lang.Resources.LNG_Menu_Encounter_Dhuum,
            Encounter.EaterOfSouls => Resources.Lang.Resources.LNG_Menu_Encounter_EaterOfSouls,
            Encounter.Ensolyss => Resources.Lang.Resources.LNG_Menu_Encounter_Ensolyss,
            Encounter.Escort => Resources.Lang.Resources.LNG_Menu_Encounter_Escort,
            Encounter.Eyes => Resources.Lang.Resources.LNG_Menu_Encounter_Eyes,
            Encounter.ForgingSteel => Resources.Lang.Resources.LNG_Menu_Encounter_ForgingSteel,
            Encounter.FraenirOfJormag => Resources.Lang.Resources.LNG_Menu_Encounter_FraenirOfJormag,
            Encounter.Freezie => Resources.Lang.Resources.LNG_Menu_Encounter_Freezie,

            // TODO:: Add locailsation to multiple golem sizes
            Encounter.StandardKittyGolem => Resources.Lang.Resources.LNG_Menu_Encounter_Golem,
            Encounter.MediumKittyGolem => Resources.Lang.Resources.LNG_Menu_Encounter_Golem,
            Encounter.LargeKittyGolem => Resources.Lang.Resources.LNG_Menu_Encounter_Golem,
            Encounter.MassiveKittyGolem => Resources.Lang.Resources.LNG_Menu_Encounter_Golem,

            Encounter.Gorseval => Resources.Lang.Resources.LNG_Menu_Encounter_Gorseval,
            Encounter.HarvestTemple => Resources.Lang.Resources.LNG_Menu_Encounter_HarvestTemple,
            Encounter.KainengOverlook => Resources.Lang.Resources.LNG_Menu_Encounter_KainengOverlook,
            Encounter.KeepConstruct => Resources.Lang.Resources.LNG_Menu_Encounter_KeepConstruct,
            Encounter.MAMA => Resources.Lang.Resources.LNG_Menu_Encounter_MAMA,
            Encounter.Mattias => Resources.Lang.Resources.LNG_Menu_Encounter_Mattias,
            Encounter.MursaatOverseer => Resources.Lang.Resources.LNG_Menu_Encounter_MursaatOverseer,
            Encounter.Qadim => Resources.Lang.Resources.LNG_Menu_Encounter_Qadim,
            Encounter.QadimThePeerless => Resources.Lang.Resources.LNG_Menu_Encounter_QadimThePeerless,
            Encounter.RiverOfSouls => Resources.Lang.Resources.LNG_Menu_Encounter_RiverOfSouls,
            Encounter.Sabetha => Resources.Lang.Resources.LNG_Menu_Encounter_Sabetha,
            Encounter.Sabir => Resources.Lang.Resources.LNG_Menu_Encounter_Sabir,
            Encounter.Samarog => Resources.Lang.Resources.LNG_Menu_Encounter_Samarog,
            Encounter.ShiverpeaksPass => Resources.Lang.Resources.LNG_Menu_Encounter_ShiverpeaksPass,
            Encounter.Siax => Resources.Lang.Resources.LNG_Menu_Encounter_Siax,
            Encounter.Skorvald => Resources.Lang.Resources.LNG_Menu_Encounter_Skorvald,
            Encounter.Slothasor => Resources.Lang.Resources.LNG_Menu_Encounter_Slothasor,
            Encounter.SoullessHorror => Resources.Lang.Resources.LNG_Menu_Encounter_SoullessHorror,
            Encounter.TwinLargos => Resources.Lang.Resources.LNG_Menu_Encounter_TwinLargos,
            Encounter.TwistedCastle => Resources.Lang.Resources.LNG_Menu_Encounter_TwistedCastle,
            Encounter.ValeGuardian => Resources.Lang.Resources.LNG_Menu_Encounter_ValeGuardian,
            Encounter.VoiceAndClawOfTheFallen => Resources.Lang.Resources.LNG_Menu_Encounter_VoiceAndClawOfTheFallen,
            Encounter.WhisperOfJormag => Resources.Lang.Resources.LNG_Menu_Encounter_WhisperOfJormag,
            Encounter.Xera => Resources.Lang.Resources.LNG_Menu_Encounter_Xera,
            Encounter.XunlaiJadeJunkyard => Resources.Lang.Resources.LNG_Menu_Encounter_XunlaiJadeJunkyard,
            _ => Resources.Lang.Resources.LNG_Menu_Encounter_Error,
        };
    }
}
