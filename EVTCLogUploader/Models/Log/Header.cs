﻿using EVTCLogUploader.Services.IO;

namespace EVTCLogUploader.Models.Log
{
    public class Header
    {
        public string BuildVerision { get; set; }
        public byte Revision { get; set; }

        public uint Id { get; set; }

        public Header(string buildVerision, byte revision, ushort id)
        {
            BuildVerision = buildVerision;
            Revision = revision;
            Id = id;
        }

        public Header(BinaryArrayReaderIO reader)
        {
            BuildVerision = reader.ReadString(12);
            Revision = reader.ReadByte();
            Id = reader.ReadUShort();
            reader.SkipBytes(1);
        }
    }
}