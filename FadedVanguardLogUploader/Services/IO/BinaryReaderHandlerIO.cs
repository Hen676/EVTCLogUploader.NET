using DynamicData;
using EVTCLogUploader.Enums;
using EVTCLogUploader.Models.Log;
using EVTCLogUploader.Utils.Determiners;
using EVTCLogUploader.Utils.GameData;
using System;
using System.Collections.Generic;

namespace EVTCLogUploader.Services.IO
{
    public class BinaryReaderHandlerIO
    {
        private readonly BinaryArrayReaderIO _reader;
        private readonly AgentItem? _userAgent;
        private readonly Header _header;
        private readonly List<AgentItem> _agentItems;
        private readonly List<SkillItem>? _skillItems;
        private readonly List<EventItem> _eventItems;

        public BinaryReaderHandlerIO(BinaryArrayReaderIO read)
        {
            _reader = read;
            _header = new Header(read);
            _agentItems = EVTCAgent();
            _userAgent = _agentItems.Find(x => x.Address == 2000);
            SkipEVTCSkill();
            _eventItems = EVTCEvent();
        }

        public BinaryReaderHandlerIO(BinaryArrayReaderIO read, bool getUserAgent)
        {
            _reader = read;
            _header = new Header(read);
            _agentItems = EVTCAgent();
            if (getUserAgent)
                _userAgent = _agentItems.Find(x => x.Address == 2000);
            _skillItems = EVTCSkill();
            _eventItems = EVTCEvent();
        }

        public TimeSpan GetLength()
        {
            EventItem? end = _eventItems.Find(x => x.IsStateChange == StateChange.LogEnd && x.Value != 0 && x.BuffDamage != 0);
            if (end == null)
                return TimeSpan.Zero;
            return DateTimeOffset.FromUnixTimeSeconds(end?.Value ?? ((int)_eventItems[^1].Time)) - GetServerDateTimeOffset();
        }

        private DateTimeOffset GetServerDateTimeOffset()
        {
            EventItem? start = _eventItems.Find(x => x.IsStateChange == StateChange.LogStart && x.Value != 0 && x.BuffDamage != 0);
            if (start == null)
                return DateTimeOffset.MinValue;
            return DateTimeOffset.FromUnixTimeSeconds(start.Value);
        }

        public DateTime GetServerDateTime() => GetServerDateTimeOffset().DateTime;

        public string GetCharcterName()
        {
            string[] names = GetNames();
            if (names.Length >= 1)
                return names[0];
            return "";
        }

        public string GetUserName()
        {
            string[] names = GetNames();
            if (names.Length >= 2)
                return names[1].Remove(names[1].Length - 1);
            return "";
        }

        private string[] GetNames() => _userAgent != null ? _userAgent.Name.Replace("\0", string.Empty).Split(":", StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();

        public Profession GetCharcterProf()
        {
            if (_userAgent == null)
                return Profession.Unknown;

            uint professionIndex = _userAgent.Prof;
            if (professionIndex > 9)
                return Profession.Unknown;
            return (Profession)professionIndex;
        }

        public Specialization GetCharcterSpec()
        {
            if (_userAgent == null)
                return Specialization.None;

            uint professionIndex = _userAgent.Prof;
            if (professionIndex > 9 || _userAgent.IsElite == 0)
                return Specialization.None;
            else if (_userAgent.IsElite == 1)
                return HotSpecializationDeterminer.Result((Profession)professionIndex);
            else
                return (Specialization)_userAgent.IsElite;
        }

        public Encounter GetEncounter() => EncounterDeterminer.Result(_header.Id, _agentItems);

        private List<uint> GetAgentIds() 
        {
            List<uint> ids = new()
            {
                _header.Id
            };
            switch (_header.Id)
            {
                case NPCIds.Berg:
                case NPCIds.Zane:
                case NPCIds.Narella:
                    ids.AddRange(new List<uint>() { NPCIds.Berg, NPCIds.Zane, NPCIds.Narella, NPCIds.TrioCagePrisoner });
                    break;
                case NPCIds.Xera:
                    ids.AddRange(new List<uint>() { NPCIds.HauntingStatue, NPCIds.XeraEnd });
                    break;
                case NPCIds.Deimos:
                    ids.AddRange(new List<uint>() { GadgetIds.ShackledPrisoner, GadgetIds.DeimosLastPhase });
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
                case NPCIds.PrototypeArsenite:
                    ids.AddRange(new List<uint>(){ NPCIds.PrototypeIndigo, NPCIds.PrototypeVermillion});
                    break;
                case NPCIds.PrototypeIndigo:
                    ids.AddRange(new List<uint>(){NPCIds.PrototypeArsenite,NPCIds.PrototypeVermillion});
                    break;
                case NPCIds.PrototypeVermillion:
                    ids.AddRange(new List<uint>(){NPCIds.PrototypeIndigo,NPCIds.PrototypeArsenite});
                    break;
                case NPCIds.PrototypeArseniteChallengeMode:
                    ids.AddRange(new List<uint>() { NPCIds.PrototypeIndigoChallengeMode, NPCIds.PrototypeVermillionChallengeMode });
                    break;
                case NPCIds.PrototypeIndigoChallengeMode:
                    ids.AddRange(new List<uint>() { NPCIds.PrototypeArseniteChallengeMode, NPCIds.PrototypeVermillionChallengeMode });
                    break;
                case NPCIds.PrototypeVermillionChallengeMode:
                    ids.AddRange(new List<uint>() { NPCIds.PrototypeIndigoChallengeMode, NPCIds.PrototypeArseniteChallengeMode });
                    break;
                case GadgetIds.TheDragonvoid:
                    ids.Add(GadgetIds.TheDragonvoidFinal);
                    break;
                case GadgetIds.TheDragonvoidFinal:
                    ids.Add(GadgetIds.TheDragonvoid);
                    break;
            }
            return ids;
        }

        private List<AgentItem> EVTCAgent()
        {
            int agentCount = _reader.ReadInt();
            List<AgentItem> val = new();
            List<uint> ids = GetAgentIds();

            for (int i = 0; i < agentCount; i++)
            {
                AgentItem agentItem = new(_reader);
                if (agentItem.IsElite == 0xffffffff && !ids.Contains((ushort)agentItem.Prof))
                    continue;
                val.Add(agentItem);
            }
            return val;
        }

        private List<SkillItem>? EVTCSkill()
        {
            int skillCount = _reader.ReadInt();
            List<SkillItem> val = new();

            for (int i = 0; i < skillCount; i++)
                val.Add(new SkillItem(_reader));
            return val;
        }

        private void SkipEVTCSkill() => _reader.SkipBytes(_reader.ReadInt() * 68);

        private List<EventItem> EVTCEvent()
        {
            List<EventItem> val = new() { new EventItem(_reader, _header.Revision) };
            while (_reader.BytesRemaining() >= 128)
            {
                EventItem eventItem = new(_reader, _header.Revision);
                if (CheckEvent(eventItem.IsStateChange))
                    val.Add(eventItem);
            }
            val.Add(new EventItem(_reader, _header.Revision));
            return val;
        }

        private static bool CheckEvent(StateChange stateChange) => stateChange == StateChange.LogStart || stateChange == StateChange.LogEnd;
    }
}

