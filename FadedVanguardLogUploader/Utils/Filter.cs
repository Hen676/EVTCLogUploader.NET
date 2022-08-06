using FadedVanguardLogUploader.Enums;
using FadedVanguardLogUploader.Models;
using System;

namespace FadedVanguardLogUploader.Utils
{
    internal class Filter
    {
        public DateTimeOffset timeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset timeOffsetMax = DateTimeOffset.MaxValue;
        public Encounter encounter = Encounter.Empty;
        public Specialization specialization = Specialization.Empty;

        public bool Predicate(ListItem i)
        {
            if (encounter == Encounter.Empty && specialization == Specialization.Empty)
                return i.CreationDate >= timeOffsetMin &&
                    i.CreationDate <= timeOffsetMax;
            if (encounter == Encounter.Empty && specialization != Specialization.Empty)
                return i.CreationDate >= timeOffsetMin &&
                    i.CreationDate <= timeOffsetMax &&
                    i.CharcterSpec == specialization;
            if(encounter != Encounter.Empty && specialization == Specialization.Empty)
                return i.CreationDate >= timeOffsetMin &&
                    i.CreationDate <= timeOffsetMax &&
                    i.Encounter == encounter;
            return i.CreationDate >= timeOffsetMin &&
                i.CreationDate <= timeOffsetMax &&
                i.Encounter == encounter &&
                i.CharcterSpec == specialization;
        }
    }
}
