using FadedVanguardLogUploader.Enums;

namespace FadedVanguardLogUploader.Models.Log
{
    public class EventItem
    {
        public EventItem(ulong time, ulong srcAgent, ulong dstAgent, int value, int buffDamage, uint overstackValue, uint skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid, Iff iff, Buff buff, Result result, Activation isActivation, BuffRemove isBuffRemove, byte isNinety, byte isFifty, byte isMoveing, StateChange isStatechange, byte isFlanking, byte isShields, byte isOffCycle)
        {
            Time = time;
            SrcAgent = srcAgent;
            DstAgent = dstAgent;
            Value = value;
            BuffDamage = buffDamage;
            OverstackValue = overstackValue;
            SkillId = skillId;
            SrcInstid = srcInstid;
            DstInstid = dstInstid;
            SrcMasterInstid = srcMasterInstid;
            Iff = iff;
            Buff = buff;
            Result = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoveing = isMoveing;
            IsStatechange = isStatechange;
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffCycle = isOffCycle;
            DstMasterInstid = 0;
        }

        public EventItem(ulong time, ulong srcAgent, ulong dstAgent, int value, int buffDamage, uint overstackValue, uint skillId, ushort srcInstid, ushort dstInstid, ushort srcMasterInstid, ushort dstMasterInstid, Iff iff, Buff buff, Result result, Activation isActivation, BuffRemove isBuffRemove, byte isNinety, byte isFifty, byte isMoveing, StateChange isStatechange, byte isFlanking, byte isShields, byte isOffCycle)
        {
            Time = time;
            SrcAgent = srcAgent;
            DstAgent = dstAgent;
            Value = value;
            BuffDamage = buffDamage;
            OverstackValue = overstackValue;
            SkillId = skillId;
            SrcInstid = srcInstid;
            DstInstid = dstInstid;
            SrcMasterInstid = srcMasterInstid;
            DstMasterInstid = dstMasterInstid;
            Iff = iff;
            Buff = buff;
            Result = result;
            IsActivation = isActivation;
            IsBuffRemove = isBuffRemove;
            IsNinety = isNinety;
            IsFifty = isFifty;
            IsMoveing = isMoveing;
            IsStatechange = isStatechange;
            IsFlanking = isFlanking;
            IsShields = isShields;
            IsOffCycle = isOffCycle;
        }

        public ulong Time { get; set; }
        public ulong SrcAgent { get; set; }
        public ulong DstAgent { get; set; }
        public int Value { get; set; }
        public int BuffDamage { get; set; }

        // Old ushort
        // New uint
        // convert old to uint
        public uint OverstackValue { get; set; }
        public uint SkillId { get; set; }

        public ushort SrcInstid { get; set; }
        public ushort DstInstid { get; set; }
        public ushort SrcMasterInstid { get; set; }
        public ushort DstMasterInstid { get; set; }

        // Old 9 Garbage bytes
        // New DstMasterInstid

        public Iff Iff { get; set; }
        public Buff Buff { get; set; }
        public Result Result { get; set; }
        public Activation IsActivation { get; set; }
        public BuffRemove IsBuffRemove { get; set; }
        public byte IsNinety { get; set; }
        public byte IsFifty { get; set; }
        public byte IsMoveing { get; set; }
        public StateChange IsStatechange { get; set; }
        public byte IsFlanking { get; set; }
        public byte IsShields { get; set; }
        public byte IsOffCycle { get; set; }

        // Old 1 pad Byte
        // New 4 pad Byte
    }
}
