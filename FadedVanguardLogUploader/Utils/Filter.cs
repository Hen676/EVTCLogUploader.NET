using EVTCLogUploader.Enums;
using EVTCLogUploader.Models;
using System;

namespace EVTCLogUploader.Utils
{
    internal class Filter
    {
        public DateTimeOffset timeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset timeOffsetMax = DateTimeOffset.MaxValue;
        // TODO Implement Spec filtering
        //public Specialization specialization = Specialization.Empty; 

        public bool Predicate(ListItem i)
        {
            return (App.Settings.FilterEncounter.Count == 0 || App.Settings.FilterEncounter.Contains(i.Encounter))
                && (App.Settings.FilterProfession.Count == 0 || App.Settings.FilterProfession.Contains(i.CharcterClass))
                && (App.Settings.FilterFileType.Count == 0 || App.Settings.FilterFileType.Contains(i.FileType))
                && !(App.Settings.ErrorFilterToggle && i.Encounter == Encounter.Unkown)
                && i.CreationDate >= timeOffsetMin
                && i.CreationDate <= timeOffsetMax;
        }
    }
}
