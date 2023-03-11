namespace EVTCLogUploader.Models.Log
{
    public class SkillItem
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public SkillItem(int id, string name)
        {
            Name = name;
            Id = id;
        }
    }
}
