using System;
using System.Text;

namespace FadedVanguardLogUploader.IO
{
    class BinaryArrayReaderIO
    {
        private readonly byte[] Reader;
        private readonly Encoding Encioding;

        private int Pos { get; set; }

        public BinaryArrayReaderIO(byte[] reader, Encoding encioding)
        {
            Reader = reader;
            Encioding = encioding;
            Pos = 0;
        }

        // 1 Byte
        public byte ReadByte()
        {
            byte val = Reader[Pos];
            Pos++;
            return val;
        }

        // X Byte Skips (X = amount)
        public void SkipBytes(int amount)
        {
            Pos += amount;
        }

        // 2 Bytes
        public short ReadShort()
        {
            short val = BitConverter.ToInt16(Reader, Pos);
            Pos += 2;
            return val;
        }

        // 2 Bytes
        public ushort ReadUShort()
        {
            return (ushort)ReadShort();
        }

        // 4 Bytes
        public int ReadInt()
        {
            int val = BitConverter.ToInt32(Reader, Pos);
            Pos += 4;
            return val;
        }

        // 4 Bytes
        public uint ReadUInt()
        {
            return (uint)ReadInt();
        }

        // 8 Bytes
        public long ReadLong()
        {
            long val = BitConverter.ToInt64(Reader, Pos);
            Pos += 8;
            return val;
        }

        // 8 Bytes
        public ulong ReadULong()
        {
            return (ulong)ReadLong();
        }

        // X Bytes (X = amount)
        public string ReadString(int amount)
        {
            string val = Encioding.GetString(Reader, Pos, amount);
            Pos += amount;
            return val;
        }

        // 1 Byte
        public bool ReadBool()
        {
            bool val = BitConverter.ToBoolean(Reader, Pos);
            Pos += 1;
            return val;
        }

        /// <summary>
        /// Returns Amount of bytes remaining
        /// </summary>
        public int BytesRemaining()
        {
            return Reader.Length - Pos;
        }
    }
}
