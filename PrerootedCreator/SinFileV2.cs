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
        public static SinFile.BlockInfoHeader[] GetBIHs(BinaryReader br)
        {
            br.BaseStream.Position = 11;
            int BIHLength = Utility.ReadIntBigEndian(br);
            if (BIHLength % _BIHSize != 0)
                throw new Exception("Woot m8, v2 too spooky");

            SinFile.BlockInfoHeader[] bihs = new SinFile.BlockInfoHeader[BIHLength / _BIHSize];
            for (int i = 0; i < bihs.Length; i++)
            {
                bihs[i].dataDest = Utility.ReadIntBigEndian(br);
                bihs[i].dataLength = Utility.ReadIntBigEndian(br);
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
