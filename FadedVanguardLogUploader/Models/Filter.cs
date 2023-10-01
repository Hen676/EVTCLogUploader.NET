﻿using EVTCLogUploader.Enums;
using EVTCLogUploader.Models.EVTCList;
using System;
using System.Collections.Generic;

namespace EVTCLogUploader.Utils
{
    public class Filter
    {
        public DateTimeOffset TimeOffsetMin = DateTimeOffset.MinValue;
        public DateTimeOffset TimeOffsetMax = DateTimeOffset.MaxValue;
        public bool ErrorFilter = true;
        public List<Encounter> FilterEncounter = new();
        List<Profession> ProfessionFilter = new();
        List<FileType> FileTypeFilter = new();


        public bool Predicate(EVTCFile i)
        {
            // TODO:: Hook up to settings service
            return (FilterEncounter.Count == 0 || FilterEncounter.Contains(i.Boss))
                && (ProfessionFilter.Count == 0 || ProfessionFilter.Contains(i.CharcterClassOfMainUser))
                && (FileTypeFilter.Count == 0 || FileTypeFilter.Contains(i.Type))
                && !(ErrorFilter && i.Boss == Encounter.Unkown)
                && i.CreationDate >= TimeOffsetMin
                && i.CreationDate <= TimeOffsetMax;
        }

        public void ClearFilters()
        {
            FilterEncounter.Clear();
            FileTypeFilter.Clear();
            ProfessionFilter.Clear();
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
            if (!FileTypeFilter.Contains(fileType))
                FileTypeFilter.Add(fileType);
            else
                FileTypeFilter.Remove(fileType);
        }

        public void EditProfessionList(Profession profession)
        {
            if (!ProfessionFilter.Contains(profession))
                ProfessionFilter.Add(profession);
            else
                ProfessionFilter.Remove(profession);
        }
    }
}