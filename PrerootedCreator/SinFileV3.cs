using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PRFCreator
{
    class SinFileV3
    {
        private const int _BIHSize = 68;

        public static int GetSinHeaderLength(BinaryReader br)
        {
            br.BaseStream.Position = 4;
            return Utility.ReadIntBigEndian(br);
        }

        public static int GetBIHSize(BinaryReader br, int SinHeaderLength)
        {
            br.BaseStream.Position = SinHeaderLength;
            int mmcflength = mmcfLength(br);
            int gptplength = GPTPLength(br);
            if ((mmcflength - gptplength) % _BIHSize != 0)
                throw new Exception("Woot m8, so spooky");

            return ((mmcflength - gptplength) / _BIHSize);
        }

        public static SinFile.BlockInfoHeader[] GetBIHs(BinaryReader br)
        {
            int size = GetBIHSize(br, GetSinHeaderLength(br));
            br.BaseStream.Position = GetBIHStart(br);
            SinFile.BlockInfoHeader[] bihs = new SinFile.BlockInfoHeader[size];
            for (int i = 0; i < size; i++)
            {
                bihs[i] = GetBIH(br);
            }
            return bihs;
        }

        public static int GetDataStart(BinaryReader br)
        {
            int headerlength = GetSinHeaderLength(br);
            //int sindataheaderlength = 16 + (GPTPLength(br) - 8) + 68 * GetBIHLength(br, sinheaderlength);
            //first SinHeaderHash
            br.BaseStream.Position = 24;
            int sindataheaderlength = Utility.ReadIntBigEndian(br);
            return (headerlength + sindataheaderlength);
        }

        public static byte[] GetUUID(BinaryReader br)
        {
            GPTPLength(br); //GPTLength function automatically seeks to UUID
            return br.ReadBytes(16);
        }

        private static int mmcfLength(BinaryReader br)
        {
            br.BaseStream.Position = GetSinHeaderLength(br);
            byte[] magicBuf = br.ReadBytes(4);
            byte[] magic = Encoding.ASCII.GetBytes("MMCF");

            if (!Utility.byteArrayCompare(magicBuf, magic))
                throw new Exception("mmcf Magic incorrect");

            return Utility.ReadIntBigEndian(br);
        }

        private static int GPTPLength(BinaryReader br)
        {
            br.BaseStream.Position = GetSinHeaderLength(br) + 8;
            byte[] magicBuf = br.ReadBytes(4);
            byte[] magic = Encoding.ASCII.GetBytes("GPTP");

            if (!Utility.byteArrayCompare(magicBuf, magic))
                throw new Exception("GPTP Magic incorrect");

            return Utility.ReadIntBigEndian(br);
        }

        private static int GetBIHStart(BinaryReader br)
        {
            int gptpsize = GPTPLength(br);
            int sinheaderlength = GetSinHeaderLength(br);
            return (sinheaderlength + 16 + (gptpsize - 8));
        }

        private static SinFile.BlockInfoHeader GetBIH(BinaryReader br)
        {
            SinFile.BlockInfoHeader bih = new SinFile.BlockInfoHeader();
            //skip for performance (is it even noticeable)
            //bih.ADDRmagic = br.ReadBytes(4);
            //bih.unknown = br.ReadBytes(4);
            br.BaseStream.Position += 8;
            bih.dataStart = Utility.ReadLongBigEndian(br);
            bih.dataLength = Utility.ReadLongBigEndian(br);
            bih.dataDest = Utility.ReadLongBigEndian(br);
            //skip for performance
            br.BaseStream.Position += 0x24;
            //bih.HashType = Utility.ReadIntBigEndian(br);
            //bih.SHA256 = br.ReadBytes(0x20);
            return bih;
        }
    }
}
