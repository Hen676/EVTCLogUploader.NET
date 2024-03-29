﻿using EVTCLogUploader.Enums;
using EVTCLogUploader.Models.Log;
using EVTCLogUploader.Utils.GameData;
using System.Collections.Generic;
using System.Linq;

namespace EVTCLogUploader.Utils.Determiners
{
    public static class EncounterDeterminer
    {
        public static Encounter Result(uint id, List<AgentItem> agents)
        {
            // Check if boss is a gadget. If so, find the gadget
            AgentItem? gadget = agents.Find(agent => (agent.Prof & 0xffff) == id && agent.Prof != id);
            if (gadget == null)
            {
                switch (id)
                {
                    #region Raids
                    case NPCIds.ValeGuardian: return Encounter.ValeGuardian;
                    case NPCIds.Gorseval: return Encounter.Gorseval;
                    case NPCIds.Sabetha: return Encounter.Sabetha;

                    case NPCIds.Slothasor: return Encounter.Slothasor;
                    case NPCIds.Berg:
                    case NPCIds.Zane:
                    case NPCIds.Narella: return Encounter.BanditTrio;
                    case NPCIds.MatthiasGabrel: return Encounter.Mattias;

                    case NPCIds.MushroomKing: return Encounter.Escort;
                    case NPCIds.KeepConstruct: return Encounter.KeepConstruct;
                    case NPCIds.HauntingStatue: return Encounter.TwistedCastle;
                    case NPCIds.Xera:
                        if (agents.Any(x => x.Address == NPCIds.HauntingStatue))
                        {
                            return Encounter.TwistedCastle;
                        }
                        return Encounter.Xera;
                    case NPCIds.XeraEnd: return Encounter.Xera;

                    case NPCIds.CairnTheIndomitable: return Encounter.Cairn;
                    case NPCIds.MursaatOverseer: return Encounter.MursaatOverseer;
                    case NPCIds.Samarog: return Encounter.Samarog;
                    case NPCIds.Deimos: return Encounter.Deimos;

                    case NPCIds.SoullessHorror: return Encounter.SoullessHorror;
                    case NPCIds.Desmina: return Encounter.RiverOfSouls;
                    case NPCIds.BrokenKing: return Encounter.BrokenKing;
                    case NPCIds.EaterOfSouls: return Encounter.EaterOfSouls;
                    case NPCIds.EyeOfJudgment:
                    case NPCIds.EyeOfFate: return Encounter.Eyes;
                    case NPCIds.Dhuum:
                        if (agents.Any(x => x.Address == NPCIds.EyeOfFate))
                        {
                            return Encounter.Eyes;
                        }
                        return Encounter.Dhuum;

                    case NPCIds.Nikare:
                    case NPCIds.Kenut: return Encounter.TwinLargos;
                    case NPCIds.Qadim: return Encounter.Qadim;
                    case NPCIds.CardinalAdina: return Encounter.Adina;
                    case NPCIds.CadinalSabir: return Encounter.Sabir;
                    case NPCIds.QadimThePeerless: return Encounter.QadimThePeerless;
                    #endregion

                    #region Strikes
                    case NPCIds.Freezie: return Encounter.Freezie;

                    case NPCIds.IcebroodConstruct: return Encounter.ShiverpeaksPass;
                    case NPCIds.VoiceOfTheFallen:
                    case NPCIds.ClawOfTheFallen: return Encounter.VoiceAndClawOfTheFallen;
                    case NPCIds.FraenirOfJormag: return Encounter.FraenirOfJormag;
                    case NPCIds.Boneskinner: return Encounter.Boneskinner;
                    case NPCIds.WhisperOfJormag: return Encounter.WhisperOfJormag;
                    case NPCIds.VariniaStormsounder: return Encounter.ColdWar;

                    case NPCIds.MaiTrin: return Encounter.AetherbladeHideout;
                    case NPCIds.Ankka: return Encounter.XunlaiJadeJunkyard;
                    case NPCIds.MinisterLi:
                    case NPCIds.MinisterLiCM: return Encounter.KainengOverlook;
                    case NPCIds.VoidAmalgamate: return Encounter.HarvestTemple;

                    case NPCIds.PrototypeArsenite:
                    case NPCIds.PrototypeArseniteChallengeMode:
                    case NPCIds.PrototypeIndigo:
                    case NPCIds.PrototypeIndigoChallengeMode:
                    case NPCIds.PrototypeVermillion:
                    case NPCIds.PrototypeVermillionChallengeMode: return Encounter.OldLionsCourt;
                    #endregion

                    #region Fractels
                    case NPCIds.MAMA: return Encounter.MAMA;
                    case NPCIds.SiaxTheCorrupted: return Encounter.Siax;
                    case NPCIds.EnsolyssOfTheEndlessTorment: return Encounter.Ensolyss;
                    case NPCIds.Skorvald: return Encounter.Skorvald;
                    case NPCIds.Artsariiv: return Encounter.Artsariiv;
                    case NPCIds.Arkk: return Encounter.Arkk;
                    case NPCIds.AiKeeperOfThePeak: return Encounter.AiKeeperOfThePeak;
                    case NPCIds.KanaxaiNM:
                    case NPCIds.KanaxaiCM: return Encounter.Kanaxai;
                    #endregion

                    #region Golems
                    case NPCIds.StandardKittyGolem: return Encounter.StandardKittyGolem;
                    case NPCIds.MediumKittyGolem: return Encounter.MediumKittyGolem;
                    case NPCIds.LargeKittyGolem: return Encounter.LargeKittyGolem;
                    case NPCIds.MassiveKittyGolem: return Encounter.MassiveKittyGolem;
                        #endregion
                }
            }
            else
            {
                switch (gadget.Prof & 0xffff)
                {
                    // Raids
                    case GadgetIds.ConjuredAmalgamate: return Encounter.ConjuredAmalgamate;
                    // Strikes
                    case GadgetIds.TheDragonvoidFinal:
                    case GadgetIds.TheDragonvoid: return Encounter.HarvestTemple;
                }
            }
            return Encounter.Unkown;
        }
    }
}
