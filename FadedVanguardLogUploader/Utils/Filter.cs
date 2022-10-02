using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models;
using System;
using System.Collections.Generic;

namespace FadedVanguardLogUploader.Utils
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
                && i.CreationDate >= timeOffsetMin 
                && i.CreationDate <= timeOffsetMax;
        }
    }
}
