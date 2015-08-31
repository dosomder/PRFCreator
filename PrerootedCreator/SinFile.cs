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

        //Credits to Androxyde, took the idea for this method from his project 'Flashtool'
        public static long GetFinalLength(FileStream fs)
        {
            fs.Position = 0;
            //ext4 superblock magic
            byte[] search = { 0x53, 0xEF };
            byte[] res = { 0, 0 };
            while (!Utility.byteArrayCompare(res, search))
            {
                if (fs.Position > 8096)
                    //some sin files are not ext4 images
                    return 0;

                if (fs.Read(res, 0, 2) != 2)
                    return 0;
            }

            fs.Position -= 0x36;
            //this is already little endian
            byte[] c = new byte[4];
            fs.Read(c, 0, 4);
            long blockcount = BitConverter.ToInt32(c, 0);
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

        public static bool isCompressed(BlockInfoHeader BIH)
        {
            if (BIH.magic == null)
                return false;
            byte[] LZ4A = Encoding.ASCII.GetBytes("LZ4A");
            if (Utility.byteArrayCompare(BIH.magic, LZ4A))
                return true;
            else
                return false;
        }

        public struct BlockInfoHeader
        {
            public byte[] magic;
            //0x4C, 0x5A, 0x34, 0x41 -> LZ4A (compressed)
            //0x41, 0x44, 0x44, 0x52 -> ADDR (uncompressed)
            public int BIHLength; //0x54 for LZ4A and 0x44 for ADDR
            public long dataStart;
            public long blockSize; //only used in LZ4A
            public long dataLength;
            public long dataDest;
            public long destLength; //only used in LZ4A, should be the same as blockSize
            //public int HashType; //not sure about that, if correct 2 = SHA256
            //public byte[] SHA256; //again SHA256 hash, probably for verifying in the destination
        }

        //see https://gist.github.com/dosomder/8ed79b26a5e063efa5ef [sinanalysis.cs] for reference
    }
}
