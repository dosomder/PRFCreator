using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.ObjectModel;

namespace PRFCreator
{
    static class SinFile
    {
        private static int[] SinSupported = { 2, 3 };

        public static byte GetSinVersion(BinaryReader br)
        {
            br.BaseStream.Position = 0;
            return br.ReadByte();
        }

        public static bool VerifySin(BinaryReader br)
        {
            int SinVer = GetSinVersion(br);
            if (!IsSupported(SinVer))
            {
                //Logger.WriteLog("Error: Only Sin file version 2 & 3 supported");
                Logger.WriteLog(String.Format("Error: Sin Version {0} not supported", SinVer));
                return false;
            }

            if(!CheckMagic(br))
            {
                Logger.WriteLog("Sin File magic incorrect");
                return false;
            }
            return true;
        }

        //Credits to Androxyde, took this method from his project 'Flashtool'
        public static long GetFinalLength(BinaryReader br, int DataStart)
        {
            //br.BaseStream.Position = GetDataStart(br);
            br.BaseStream.Position = DataStart;
            byte[] search = { 0x53, 0xEF };
            int i = 0;
            while (!Utility.byteArrayCompare(br.ReadBytes(2), search))
            {
                i += 2;
                if (i > 4096)
                    //some sin files are not ext4 images
                    return 0;
            }

            br.BaseStream.Position -= 0x36;
            //this is already little endian
            long blockcount = br.ReadInt32();
            return (blockcount * 4 * 1024);
        }

        public static int GetDataStart(BinaryReader br)
        {
            int SinVer = GetSinVersion(br);
            switch (SinVer)
            {
                case 2:
                    return SinFileV2.GetDataStart(br);
                case 3:
                    return SinFileV3.GetDataStart(br);
                default:
                    throw new Exception("GetDataStart: Unknown Version (" + SinVer + ")");
            }
        }

        private static bool IsSupported(int SinVersion)
        {
            foreach (int ver in SinSupported)
            {
                if (ver == SinVersion)
                    return true;
            }
            return false;
        }

        private static bool CheckMagic(BinaryReader br)
        {
            //Sin V2 does not have a magic
            if (GetSinVersion(br) == 2)
                return true;

            byte[] magicBuf = br.ReadBytes(3);
            byte[] magic = Encoding.ASCII.GetBytes("SIN");
            if (Utility.byteArrayCompare(magicBuf, magic))
                return true;
            else
                return false;
        }

        public struct BlockInfoHeader
        {
            //public byte[] ADDRmagic; //0x41, 0x44, 0x44, 0x52 -> ADDR
            //public byte[] unknown;
            public long dataStart;
            public long dataLength;
            public long dataDest; //Address of destination in new file, everything else in the file is 0xFF (kind of 'compression')
            //public int HashType; //not sure about that, if correct 2 = SHA256
            //public byte[] SHA256; //again SHA256 hash, probably for verifying in the destination
        }

        //see https://gist.github.com/dosomder/8ed79b26a5e063efa5ef [sinanalysis.cs] for reference
    }
}
