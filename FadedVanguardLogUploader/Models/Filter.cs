using EVTCLogUploader.Enums;
using EVTCLogUploader.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EVTCLogUploader.Utils
{
    public class Filter
    {
        public DateTimeOffset TimeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset TimeOffsetMax = DateTimeOffset.MaxValue;
        public bool ErrorFilter = true;
        public List<Encounter> FilterEncounter = new();

        private readonly List<Profession> _professionFilter = new();
        private readonly List<FileType> _fileTypeFilter = new();


        public bool Predicate(EVTCFile i)
        {
            return (FilterEncounter.Count == 0 || FilterEncounter.Contains(i.Boss))
                && (_professionFilter.Count == 0 || _professionFilter.Contains(i.CharcterClassOfMainUser))
                && (_fileTypeFilter.Count == 0 || _fileTypeFilter.Contains(i.Type))
                && !(ErrorFilter && i.Boss == Encounter.Unkown)
                && i.CreationDate >= TimeOffsetMin
                && i.CreationDate <= TimeOffsetMax;
        }

        public void ClearFilters()
        {
            TimeOffsetMin = DateTimeOffset.MinValue;
            TimeOffsetMax = DateTimeOffset.MaxValue;
            FilterEncounter.Clear();
            _fileTypeFilter.Clear();
            _professionFilter.Clear();
        }

        public void EditEncounterList(List<EncounterNode> encounters)
        {
            List<Encounter> list = GetEncounters(encounters);

            FilterEncounter.Clear();
            FilterEncounter.AddRange(list);
        }

        private List<Encounter> GetEncounters(List<EncounterNode> encounters) 
        {
            List<Encounter> list = new();
            encounters.ForEach(e => {
                if (e.SubNodes != null) {
                    list.AddRange(GetEncounters(e.SubNodes.ToList()));
                }
                if (e.Boss.HasValue) {
                    list.Add(e.Boss.Value);
                }
            });
            return list;
        }

        public void EditEncounterList(Encounter encounter)
        {
            if (!FilterEncounter.Contains(encounter))
                FilterEncounter.Add(encounter);
            else
                FilterEncounter.Remove(encounter);
        }

        public void EditFileTypeList(FileType fileType)
        {
            if (!_fileTypeFilter.Contains(fileType))
                _fileTypeFilter.Add(fileType);
            else
                _fileTypeFilter.Remove(fileType);
        }

        public void EditProfessionList(Profession profession)
        {
            if (!_professionFilter.Contains(profession))
                _professionFilter.Add(profession);
            else
                _professionFilter.Remove(profession);
        }
    }
}
