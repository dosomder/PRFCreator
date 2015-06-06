using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PRFCreator
{
    class SinFileV3
    {
        private const int _BIHSize = 68; //0x44
        private const int _BIHSizeCompressed = 84; //0x54

        public static int GetSinHeaderLength(BinaryReader br)
        {
            br.BaseStream.Position = 4;
            return Utility.ReadIntBigEndian(br);
        }

        public static List<SinFile.BlockInfoHeader> GetBIHs(BinaryReader br)
        {
            int dataStart = GetDataStart(br);
            br.BaseStream.Position = GetBIHStart(br);
            List<SinFile.BlockInfoHeader> bihs = new List<SinFile.BlockInfoHeader>();

            while (br.BaseStream.Position < dataStart)
                bihs.Add(GetBIH(br));

            return bihs;
        }

        public static int GetDataStart(BinaryReader br)
        {
            int headerlength = GetSinHeaderLength(br);
            int mmcflength = mmcfLength(br);
            return (headerlength + mmcflength + 4 /*size of GPTP magic*/ + 4 /*size of mmcfLength*/);
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
            bih.magic = br.ReadBytes(4);
            bih.BIHLength = Utility.ReadIntBigEndian(br);
            if (SinFile.isCompressed(bih) && bih.BIHLength != _BIHSizeCompressed)
                throw new FormatException("woot, compressed bih is spooky");

            bih.dataStart = Utility.ReadLongBigEndian(br);

            if (SinFile.isCompressed(bih))
                bih.blockSize = Utility.ReadLongBigEndian(br);
            bih.dataLength = Utility.ReadLongBigEndian(br);
            bih.dataDest = Utility.ReadLongBigEndian(br);

            if (SinFile.isCompressed(bih))
                bih.destLength = Utility.ReadLongBigEndian(br);

            //skip for performance
            br.BaseStream.Position += 0x24;
            //bih.HashType = Utility.ReadIntBigEndian(br);
            //bih.SHA256 = br.ReadBytes(0x20);
            return bih;
        }
    }
}
