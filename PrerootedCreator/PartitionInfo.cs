using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;

namespace PRFCreator
{
    class PartitionInfo
    {
        //public static PartInfo[] pinfo = null;
        //public static bool UsingUUID { get { return (pinfo != null); } }
        private static bool _UsingUUID = true;
        public static bool UsingUUID 
        { 
            get { return _UsingUUID; }
            set 
            {
                _UsingUUID = value;
                if (value)
                {
                    Zipping.RenameInZip("flashable.zip", "META-INF/com/google/android/updater-script", "META-INF/com/google/android/updater-script-names");
                    Zipping.RenameInZip("flashable.zip", "META-INF/com/google/android/updater-script-uuid", "META-INF/com/google/android/updater-script");
                }
                else
                {
                    Zipping.RenameInZip("flashable.zip", "META-INF/com/google/android/updater-script", "META-INF/com/google/android/updater-script-uuid");
                    Zipping.RenameInZip("flashable.zip", "META-INF/com/google/android/updater-script-names", "META-INF/com/google/android/updater-script");
                }
            }
        }

        public static byte[] ReadSinUUID(string sinfile)
        {
            using (Stream stream = new FileStream(sinfile, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(stream))
            {
                if (SinFile.GetSinVersion(br) == 2)
                    return null;
                return SinFileV3.GetUUID(br);
            }
        }

        /*public static void ReadPartInfo(BackgroundWorker worker, string ftffile)
        {
            if (!Zipping.ExistsInZip("partition-image.sin", ftffile))
            {
                Logger.WriteLog("Info: No partition-image.sin found in ftf. Using default partition names");
                return;
            }

            Zipping.UnzipFile(worker, ftffile, "partition-image.sin", string.Empty, System.IO.Path.GetTempPath(), false);

            using (Stream stream = new FileStream(System.IO.Path.GetTempPath() + "\\partition-image.sin", FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new BinaryReader(stream))
                ParsePInfo(br);
        }

        private static void ParsePInfo(BinaryReader br)
        {
            int skip = SinFile.GetSinHeaderLength(br);
            br.BaseStream.Position = skip + 0xC; //this 0xC is some unknown data
            int partitionCount = Utility.ReadIntBigEndian(br);
            if (partitionCount < 2 || //there should be atleast bootloader/kernel/etc
                partitionCount > 40) //more than 40 partitions is highly unlikely
            {
                Logger.WriteLog("Info: Could not read partition count. Using default partition names");
                return;
            }

            pinfo = new PartInfo[partitionCount];
            for (int i = 0; i < partitionCount; i++)
            {
                br.BaseStream.Position += 16;
                pinfo[i].UUID = br.ReadBytes(16);
                br.BaseStream.Position += 24;
                byte[] pname = br.ReadBytes(72);
                pinfo[i].PartitionName = Encoding.Unicode.GetString(pname);
            }
        }

        public struct PartInfo
        {
            public byte[] UUID;
            public string PartitionName;
        }*/
    }
}

/*
GUID partition entry format Offset 	Length 	Contents
0 (0x00) 	16 bytes 	Partition type GUID
16 (0x10) 	16 bytes 	Unique partition GUID <-- uuid
32 (0x20) 	8 bytes 	First LBA (little endian)
40 (0x28) 	8 bytes 	Last LBA (inclusive, usually odd)
48 (0x30) 	8 bytes 	Attribute flags (e.g. bit 60 denotes read-only)
56 (0x38) 	72 bytes 	Partition name (36 UTF-16LE code units)
	128 bytes total

* taken from wikipedia
*/