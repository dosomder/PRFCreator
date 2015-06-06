using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace PRFCreator
{
    class SinFileV2
    {
        //V2 doesn't really have a BIH, it's more like a SinHeaderHash
        private const int _BIHSize = 41;

        public static int GetSinHeaderLength(BinaryReader br)
        {
            br.BaseStream.Position = 2;
            return Utility.ReadIntBigEndian(br);
        }

        //roughly taken from flashtool
        public static List<SinFile.BlockInfoHeader> GetBIHs(BinaryReader br)
        {
            br.BaseStream.Position = 11;
            int BIHLength = Utility.ReadIntBigEndian(br);
            if (BIHLength % _BIHSize != 0)
                throw new Exception("Woot m8, v2 too spooky");

            List<SinFile.BlockInfoHeader> bihs = new List<SinFile.BlockInfoHeader>();
            for (int i = 0; i < (BIHLength / _BIHSize); i++)
            {
                SinFile.BlockInfoHeader bih = new SinFile.BlockInfoHeader();
                bih.dataDest = Utility.ReadIntBigEndian(br);
                bih.dataLength = Utility.ReadIntBigEndian(br);
                bihs.Add(bih);
                br.BaseStream.Position += 0x21; //SHA256 and 1 unknown byte
            }

            return bihs;
        }

        public static int GetDataStart(BinaryReader br)
        {
            int headerlength = GetSinHeaderLength(br);
            return headerlength;
        }
    }
}
