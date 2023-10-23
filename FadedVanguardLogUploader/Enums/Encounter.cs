namespace EVTCLogUploader.Enums
{
    public enum Encounter
    {
        // Default + Filter
        Empty, Unkown,

        // Raids
        // W1
        ValeGuardian = 10_000, Gorseval, Sabetha,
        // W2
        Slothasor, BanditTrio, Mattias,
        // W3
        Escort, KeepConstruct, TwistedCastle, Xera,
        // W4
        Cairn, MursaatOverseer, Samarog, Deimos,
        // W5
        SoullessHorror, RiverOfSouls, BrokenKing, EaterOfSouls, Eyes, Dhuum,
        // W6
        ConjuredAmalgamate, TwinLargos, Qadim,
        // W7
        Adina, Sabir, QadimThePeerless,


        // Strikes
        // Holiday
        Freezie = 20_000,
        // Icebrood
        ShiverpeaksPass,
        VoiceAndClawOfTheFallen, FraenirOfJormag, Boneskinner,
        WhisperOfJormag, ForgingSteel, ColdWar,
        // EOD
        AetherbladeHideout, XunlaiJadeJunkyard, KainengOverlook, HarvestTemple,
        // LWS 1
        OldLionsCourt,
        // SOFO
        CosmicObservatory,
        TempleOfFebe,


        // Golem
        StandardKittyGolem = 30_000,
        MediumKittyGolem,
        LargeKittyGolem,
        MassiveKittyGolem,

        // Fractels
        MAMA = 40_000, Siax, Ensolyss,
        Skorvald, Artsariiv, Arkk,
        AiKeeperOfThePeak,
        Kanaxai
    }
}
