using EVTCLogUploader.Enums;

namespace EVTCLogUploader.Utils.Determiners
{
    public static class HotSpecializationDeterminer
    {
        public static Specialization Result(Profession prof) => prof switch
        {
            Profession.Guardian => Specialization.Dragonhunter,
            Profession.Warrior => Specialization.Berserker,
            Profession.Engineer => Specialization.Scrapper,
            Profession.Ranger => Specialization.Druid,
            Profession.Thief => Specialization.Daredevil,
            Profession.Elementalist => Specialization.Tempest,
            Profession.Mesmer => Specialization.Chronomancer,
            Profession.Necromancer => Specialization.Reaper,
            Profession.Revenant => Specialization.Herald,
            _ => Specialization.None
        };
    }
}
