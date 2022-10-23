using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models.Log;
using FadedVanguardLogUploader.Utils.Determiners;
using FadedVanguardLogUploader.Utils.GameData;
using System;
using System.Collections.Generic;

namespace FadedVanguardLogUploader.IO
{
    public class BinaryReaderHandlerIO
    {
        private readonly BinaryArrayReaderIO reader;
        private readonly Encounter encounter;
        private readonly TimeSpan length;
        private readonly DateTimeOffset SeverStartTime;
        private readonly string charcterName;
        private readonly string userName;
        private readonly Profession charcterProf;
        private readonly Specialization charcterSpec;

        public BinaryReaderHandlerIO(BinaryArrayReaderIO rea)
        {
            reader = rea;
            Header header = EVTCHeader();
            List<AgentItem> agents = EVTCAgent(header.Id);
            encounter = EncounterDeterminer.Result(header.Id, agents);

            AgentItem? user = agents.Find(x => x.Address == 2000);
            if (user != null)
            {
                string[] names = user.Name.Replace("\0", string.Empty).Split(":", StringSplitOptions.RemoveEmptyEntries);
                charcterName = names[0];
                userName = names[1].Remove(names[1].Length - 1);

                uint professionIndex = user.Prof;
                if (professionIndex > 9)
                {
                    charcterProf = Profession.Unknown;
                    charcterSpec = Specialization.None;
                }
                else
                {
                    charcterProf = (Profession)professionIndex;
                    if (user.IsElite == 0)
                    {
                        charcterSpec = Specialization.None;
                    }
                    else if (user.IsElite == 1)
                    {
                        charcterSpec = HotSpecializationDeterminer.Result(charcterProf);
                    }
                    else
                    {
                        charcterSpec = (Specialization)user.IsElite;
                    }
                }
            }
            else
            {
                charcterName = "";
                userName = "";
                charcterProf = Profession.Unknown;
                charcterSpec = Specialization.None;
            }
            SkipEVTCSkill();
            List<EventItem> events = EVTCEvent(header.Revision);
            SeverStartTime = DateTimeOffset.MinValue;
            var start = events.Find(x => x.IsStatechange == StateChange.LogStart && x.Value != 0 && x.BuffDamage != 0);
            var end = events.Find(x => x.IsStatechange == StateChange.LogEnd && x.Value != 0 && x.BuffDamage != 0);
            if (start == null)
            {
                length = TimeSpan.Zero;
                return;
            }
            SeverStartTime = DateTimeOffset.FromUnixTimeSeconds(start.Value);
            length = DateTimeOffset.FromUnixTimeSeconds(end?.Value ?? ((int)events[^1].Time)) - SeverStartTime;
        }

        private Header EVTCHeader()
        {
            string buildVersision = reader.ReadString(12);
            byte revision = reader.ReadByte();
            ushort id = reader.ReadUShort();
            reader.SkipBytes(1);

            return new Header(buildVersision, revision, id);
        }

        private List<AgentItem> EVTCAgent(uint id)
        {
            int agentCount = reader.ReadInt();
            List<AgentItem> val = new();
            List<uint> ids = new();
            ids.Add(id);

            switch (id)
            {
                case NPCIds.Berg:
                case NPCIds.Zane:
                case NPCIds.Narella:
                    ids.Add(NPCIds.Berg);
                    ids.Add(NPCIds.Zane);
                    ids.Add(NPCIds.Narella);
                    ids.Add(NPCIds.TrioCagePrisoner);
                    break;
                case NPCIds.Xera:
                    ids.Add(NPCIds.HauntingStatue);
                    ids.Add(NPCIds.XeraEnd);
                    break;
                case NPCIds.Deimos:
                    ids.Add(GadgetIds.ShackledPrisoner);
                    ids.Add(GadgetIds.DeimosLastPhase);
                    break;
                case NPCIds.Dhuum:
                    ids.Add(NPCIds.EyeOfFate);
                    break;
                case NPCIds.Nikare:
                    ids.Add(NPCIds.Kenut);
                    break;
                case NPCIds.Kenut:
                    ids.Add(NPCIds.Nikare);
                    break;
                case NPCIds.VoiceOfTheFallen:
                    ids.Add(NPCIds.ClawOfTheFallen);
                    break;
                case NPCIds.ClawOfTheFallen:
                    ids.Add(NPCIds.VoiceOfTheFallen);
                    break;
                case GadgetIds.TheDragonvoid:
                    ids.Add(GadgetIds.TheDragonvoidFinal);
                    break;
                case GadgetIds.TheDragonvoidFinal:
                    ids.Add(GadgetIds.TheDragonvoid);
                    break;
            }

            for (int i = 0; i < agentCount; i++)
            {
                ulong address = reader.ReadULong();
                uint prof = reader.ReadUInt();
                uint isElite = reader.ReadUInt();
                if (isElite != 0xffffffff || (isElite == 0xffffffff && (!ids.Contains((ushort)prof))))
                {
                    reader.SkipBytes(80);
                    continue;
                }
                short toughness = reader.ReadShort();
                short concentration = reader.ReadShort();
                short healing = reader.ReadShort();
                short hitboxWidth = reader.ReadShort();
                short condition = reader.ReadShort();
                short hitboxHeight = reader.ReadShort();
                string name = reader.ReadString(68);
                val.Add(new AgentItem(address, prof, isElite, toughness, concentration, condition, healing, hitboxWidth, hitboxHeight, name));
            }

            return val;
        }

        public TimeSpan GetLength()
        {
            return length;
        }

        public string GetCharcterName()
        {
            return charcterName;
        }

        public string GetUserName()
        {
            return userName;
        }

        public Profession GetCharcterProf()
        {
            return charcterProf;
        }

        public Specialization GetCharcterSpec()
        {
            return charcterSpec;
        }

        public Encounter GetEncounter()
        {
            return encounter;
        }

        public DateTime GetServerDateTime()
        {
            return SeverStartTime.DateTime;
        }

        private void SkipEVTCSkill()
        {
            int skillCount = reader.ReadInt();
            reader.SkipBytes(skillCount * 68);
        }

        private List<EventItem> EVTCEvent(byte revision)
        {
            List<EventItem> val = new();
            val = revision switch
            {
                0 => EventRevision0(val),
                1 => EventRevision1(val),
                _ => throw new ArgumentException("Only Revision 0 and 1 supported"),
            };
            return val;
        }

        private List<EventItem> EventRevision0(List<EventItem> val)
        {
            val.Add(ReadEventItem0());
            while (reader.BytesRemaining() >= 128)
            {
                EventItem eventItem = ReadEventItem0();
                if (CheckEvent(eventItem.IsStatechange))
                    val.Add(eventItem);
            }
            val.Add(ReadEventItem0());
            return val;
        }

        private EventItem ReadEventItem0()
        {
            ulong time = reader.ReadULong();
            ulong srcAgent = reader.ReadULong();
            ulong dstAgent = reader.ReadULong();
            int value = reader.ReadInt();
            int buffDmg = reader.ReadInt();
            uint overstackValue = reader.ReadUShort();
            uint skillId = reader.ReadUShort();
            ushort srcInstid = reader.ReadUShort();
            ushort dstInstid = reader.ReadUShort();
            ushort srcMasterInstid = reader.ReadUShort();
            reader.SkipBytes(9);
            Iff iff = (Iff)reader.ReadByte();
            Buff buff = (Buff)reader.ReadByte();
            Result result = (Result)reader.ReadByte();
            Activation isActivation = (Activation)reader.ReadByte();
            BuffRemove isBuffRemove = (BuffRemove)reader.ReadByte();
            byte isNinety = reader.ReadByte();
            byte isFifty = reader.ReadByte();
            byte isMoveing = reader.ReadByte();
            StateChange isStateChange = (StateChange)reader.ReadByte();
            byte isFlanking = reader.ReadByte();
            byte isSheilds = reader.ReadByte();
            byte isOffCycle = reader.ReadByte();
            reader.SkipBytes(1);
            return new EventItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId, srcInstid, dstInstid, srcMasterInstid, iff, buff, result, isActivation, isBuffRemove, isNinety, isFifty, isMoveing, isStateChange, isFlanking, isSheilds, isOffCycle);
        }

        private List<EventItem> EventRevision1(List<EventItem> val)
        {
            val.Add(ReadEventItem1());
            while (reader.BytesRemaining() >= 128)
            {
                EventItem eventItem = ReadEventItem1();
                if (CheckEvent(eventItem.IsStatechange))
                    val.Add(eventItem);
            }
            val.Add(ReadEventItem1());
            return val;
        }

        private EventItem ReadEventItem1()
        {
            ulong time = reader.ReadULong();
            ulong srcAgent = reader.ReadULong();
            ulong dstAgent = reader.ReadULong();
            int value = reader.ReadInt();
            int buffDmg = reader.ReadInt();
            uint overstackValue = reader.ReadUInt();
            uint skillId = reader.ReadUInt();
            ushort srcInstid = reader.ReadUShort();
            ushort dstInstid = reader.ReadUShort();
            ushort srcMasterInstid = reader.ReadUShort();
            ushort dstMasterInstid = reader.ReadUShort();
            Iff iff = (Iff)reader.ReadByte();
            Buff buff = (Buff)reader.ReadByte();
            Result result = (Result)reader.ReadByte();
            Activation isActivation = (Activation)reader.ReadByte();
            BuffRemove isBuffRemove = (BuffRemove)reader.ReadByte();
            byte isNinety = reader.ReadByte();
            byte isFifty = reader.ReadByte();
            byte isMoveing = reader.ReadByte();
            StateChange isStateChange = (StateChange)reader.ReadByte();
            byte isFlanking = reader.ReadByte();
            byte isSheilds = reader.ReadByte();
            byte isOffCycle = reader.ReadByte();
            reader.SkipBytes(4);
            return new EventItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId, srcInstid, dstInstid, srcMasterInstid, dstMasterInstid, iff, buff, result, isActivation, isBuffRemove, isNinety, isFifty, isMoveing, isStateChange, isFlanking, isSheilds, isOffCycle);
        }

        private static bool CheckEvent(StateChange stateChange)
        {
            return stateChange == StateChange.LogStart || stateChange == StateChange.LogEnd;
        }
    }
}

