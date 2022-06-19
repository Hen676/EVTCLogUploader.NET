using CsvHelper.Configuration.Attributes;
using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.IO;
using FadedVanguardLogUploader.Models.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace FadedVanguardLogUploader.Models
{
    public class ListItem
    {
        public string FullPath { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; } = DateTime.MinValue;
        public string BossName { get; set; } = string.Empty;
        public bool Success { get; set; } = false;
        public string UserName { get; set; } = string.Empty;
        public string CharcterName { get; set; } = string.Empty;
        [Ignore]
        public bool IsSelected { get; set; }

#if DEBUG
        [Ignore]
        private static int i = 0;
#endif

        public ListItem()
        {
            IsSelected = false;
        }

        public ListItem(string path)
        {
            FullPath = path;
            FileInfo fileInfo = new FileInfo(FullPath);
            Name = fileInfo.Name;
            CreationDate = fileInfo.CreationTime;
            IsSelected = false;
            LoadData();
#if DEBUG
            Name += i;
            i++;
#endif
        }

        public void LoadData()
        {
            BinaryArrayReaderIO reader;

            // Uncompress EVTC if needed
            if (FullPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
                || FullPath.EndsWith(".zevtc", StringComparison.OrdinalIgnoreCase))
            {
                using ZipArchive zip = ZipFile.Open(FullPath, ZipArchiveMode.Read);
                using Stream data = zip.Entries[0].Open();
                var bytes = new byte[zip.Entries[0].Length];
                data.Read(bytes, 0, bytes.Length);
                if (bytes.Length == 0)
                    return;
                reader = new BinaryArrayReaderIO(bytes, new UTF8Encoding());
            }
            else
                reader = new BinaryArrayReaderIO(File.ReadAllBytes(FullPath), new UTF8Encoding());

            // Read Log
            Header header = EVTCHeader(reader);
            List<AgentItem> agents = EVTCAgent(reader, header.Id);
            SkipEVTCSkill(reader);
            List<EventItem> events = EVTCEvent(reader, header.Revision);
            AgentItem? boss = agents.Find(x => x.Prof == header.Id);
            if (boss != null)
            {
                BossName = boss.Name.Replace("\0", string.Empty);

                EventItem? killed = events.FindLast(x => x.IsStatechange == StateChange.ChangeDead && x.SrcInstid == boss.Address);

                if (killed != null)
                {
                    Success = true;
                }
            }

            AgentItem? user = agents.Find(x => x.Address == 2000);
            if (user != null)
            {
                string[] names = user.Name.Replace("\0", string.Empty).Split(":", StringSplitOptions.RemoveEmptyEntries);
                CharcterName = names[0];
                UserName = names[1].Remove(names[1].Length - 1);
            }
        }

        private static Header EVTCHeader(BinaryArrayReaderIO reader)
        {
            string buildVersision = reader.ReadString(12);
            byte revision = reader.ReadByte();
            ushort id = reader.ReadUShort();
            reader.SkipBytes(1);

            return new Header(buildVersision, revision, id);
        }

        private static List<AgentItem> EVTCAgent(BinaryArrayReaderIO reader, ushort bossid)
        {
            int agentCount = reader.ReadInt();
            List<AgentItem> val = new();

            for (int i = 0; i < agentCount; i++)
            {
                ulong address = reader.ReadULong();
                uint prof = reader.ReadUInt();
                uint isElite = reader.ReadUInt();
                if (isElite == 0xffffffff && prof != bossid)
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

        private static void SkipEVTCSkill(BinaryArrayReaderIO reader)
        {
            int skillCount = reader.ReadInt();
            reader.SkipBytes(skillCount * 68);
        }

        private List<EventItem> EVTCEvent(BinaryArrayReaderIO reader, byte revision)
        {
            List<EventItem> val = new();
            val = revision switch
            {
                0 => EventRevision0(reader, val),
                1 => EventRevision1(reader, val),
                _ => throw new ArgumentException("Only Revsioin 0 and 1 supported"),
            };
            return val;
        }

        private static List<EventItem> EventRevision0(BinaryArrayReaderIO reader, List<EventItem> val)
        {
            while (reader.BytesRemaining() > 64)
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
                if (CheckEvent(isStateChange))
                    val.Add(new EventItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId, srcInstid, dstInstid, srcMasterInstid, iff, buff, result, isActivation, isBuffRemove, isNinety, isFifty, isMoveing, isStateChange, isFlanking, isSheilds, isOffCycle));
            }
            return val;
        }

        private static List<EventItem> EventRevision1(BinaryArrayReaderIO reader, List<EventItem> val)
        {
            while (reader.BytesRemaining() > 64)
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
                if (CheckEvent(isStateChange))
                    val.Add(new EventItem(time, srcAgent, dstAgent, value, buffDmg, overstackValue, skillId, srcInstid, dstInstid, srcMasterInstid, dstMasterInstid, iff, buff, result, isActivation, isBuffRemove, isNinety, isFifty, isMoveing, isStateChange, isFlanking, isSheilds, isOffCycle));
            }
            return val;
        }

        private static bool CheckEvent(StateChange stateChange)
        {
            return stateChange == StateChange.ChangeDead
                || stateChange == StateChange.HealthUpdate;
        }
    }
}
