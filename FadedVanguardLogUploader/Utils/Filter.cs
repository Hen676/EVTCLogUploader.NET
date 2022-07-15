using FadedVanguardLogUploader.Models;
using System;

namespace FadedVanguardLogUploader.Util
{
    internal class Filter
    {
        public DateTimeOffset timeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset timeOffsetMax = DateTimeOffset.MaxValue;

        public bool Predicate(ListItem i)
        {
            return i.CreationDate >= timeOffsetMin &&
                i.CreationDate <= timeOffsetMax;
        }
    }
}
