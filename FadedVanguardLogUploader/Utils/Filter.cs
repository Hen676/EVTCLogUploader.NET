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
        public List<Encounter> encounter = new();
        // TODO Implement Spec filtering
        //public Specialization specialization = Specialization.Empty; 

        public bool Predicate(ListItem i)
        {
            return (encounter.Count == 0 || encounter.Contains(i.Encounter)) 
                && i.CreationDate >= timeOffsetMin 
                && i.CreationDate <= timeOffsetMax;
        }
    }
}
