using FadedVanguardLogUploader.Models;
using System;

namespace FadedVanguardLogUploader.Util
{
    internal class Filter
    {
        public DateTimeOffset timeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset timeOffsetMax = DateTimeOffset.MaxValue;

        public bool Predicate(ListEVTC i)
        {
            return i.FileInfo.CreationTime >= timeOffsetMin &&
                i.FileInfo.CreationTime <= timeOffsetMax;
        }
    }
}
