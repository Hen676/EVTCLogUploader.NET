namespace EVTCLogUploader.Models.Log
{
    public class AgentItem
    {
        public ulong Address { get; set; }

        public uint Prof { get; set; }
        public uint IsElite { get; set; }

        public short Toughness { get; set; }
        public short Concentration { get; set; }
        public short Condition { get; set; }
        public short Healing { get; set; }
        public short HitboxWidth { get; set; }
        public short HitboxHeight { get; set; }

        public string Name { get; set; }

        public AgentItem(ulong address, uint prof, uint isElite, short toughness, short concentration, short condition, short healing, short hitboxWidth, short hitboxHeight, string name)
        {
            Address = address;
            Prof = prof;
            IsElite = isElite;
            Toughness = toughness;
            Concentration = concentration;
            Condition = condition;
            Healing = healing;
            HitboxWidth = hitboxWidth;
            HitboxHeight = hitboxHeight;
            Name = name;
        }
    }
}
