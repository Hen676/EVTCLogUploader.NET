using EVTCLogUploader.Enums;
using EVTCLogUploader.Utils.Determiners;
using System.Collections.ObjectModel;

namespace EVTCLogUploader.Models
{
    public class EncounterNode
    {
        public ObservableCollection<EncounterNode>? SubNodes { get; }
        public string Title { get; }
        public Encounter? Boss { get; set; }

        public EncounterNode(Encounter boss)
        {
            Boss = boss;
            Title = EncounterNameDeterminer.Result(boss);
        }

        public EncounterNode(string title, ObservableCollection<EncounterNode> subNodes)
        {
            Title = title;
            SubNodes = subNodes;
        }
    }
}
