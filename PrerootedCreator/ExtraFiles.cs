using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;

namespace PRFCreator
{
    static class ExtraFiles
    {
        public static void AddExtraFiles(BackgroundWorker worker, string name, string ftffile)
        {
            switch (name)
            {
                case "kernel":
                    AddKernel(worker, ftffile);
                    break;
                case "fotakernel":
                    AddFOTAKernel(worker, ftffile);
                    break;
                case "modem":
                    AddModem(worker, ftffile);
                    break;
                case "ltalabel":
                    AddLTALabel(worker, ftffile);
                    break;
            }
        }

        private static void AddKernel(BackgroundWorker worker, string ftffile)
        {
            ExtractAndAdd(worker, "kernel", ".elf", ftffile, "boot");
            ExtractAndAdd(worker, "rpm", ".elf", ftffile);
        }

        private static void AddFOTAKernel(BackgroundWorker worker, string ftffile)
        {
            ExtractAndAdd(worker, "fotakernel", ".elf", ftffile);
        }

        private static void AddLTALabel(BackgroundWorker worker, string ftffile)
        {
            string ltalname = Zipping.ZipGetFullname(ftffile, "elabel*.sin");
            if (string.IsNullOrEmpty(ltalname))
            {
                Logger.WriteLog("   Error: Could not find LTALabel in FTF");
                return;
            }
            ExtractAndAdd(worker, Path.GetFileNameWithoutExtension(ltalname), ".ext4", ftffile, "ltalabel");
        }

        private static void AddModem(BackgroundWorker worker, string ftffile)
        {
            //different firmwares have different sin files
            string[] mdms = { "amss_fsg", "amss_fs_3", "modem" };
            foreach (string modem in mdms)
            {
                if (Zipping.ExistsInZip(ftffile, modem + ".sin"))
                {
                    ExtractAndAdd(worker, modem, string.Empty, ftffile);
                    break;
                }
            }
            ExtractAndAdd(worker, "amss_fs_1", string.Empty, ftffile);
            ExtractAndAdd(worker, "amss_fs_2", string.Empty, ftffile);
        }

        private static void ExtractAndAdd(BackgroundWorker worker, string name, string extension, string ftffile, string AsFilename = "")
        {
            if (Zipping.ExistsInZip(ftffile, name + ".sin") == false)
            {
                OnError(name, AsFilename);
                return;
            }

            Zipping.UnzipFile(worker, ftffile, name + ".sin", string.Empty, System.IO.Path.GetTempPath(), false);
            if (File.Exists(System.IO.Path.GetTempPath() + "\\" + name + ".sin"))
            {
                //Logger.WriteLog("Adding " + name + " to zip");
                Logger.WriteLog("   " + name);
                SinExtract.ExtractSin(worker, System.IO.Path.GetTempPath() + "\\" + name + ".sin", System.IO.Path.GetTempPath() + "\\" + name + extension, false);

                if (PartitionInfo.UsingUUID)
                {
                    byte[] UUID = PartitionInfo.ReadSinUUID(System.IO.Path.GetTempPath() + "\\" + name + ".sin");
                    Utility.ScriptSetUUID(worker, (AsFilename == "" ? name : AsFilename), UUID);
                }

                File.Delete(System.IO.Path.GetTempPath() + "\\" + name + ".sin");
                Zipping.AddToZip(worker, "flashable.zip", System.IO.Path.GetTempPath() + "\\" + name + extension, (AsFilename == "" ? name : AsFilename) + extension, false);
            }
        }

        private static void OnError(string name, string AsFilename = "")
        {
            switch (name)
            {
                case "rpm":
                    //rpm seems to be missing in older firmwares, so we don't display it as error
                    Logger.WriteLog("   Info: Could not find " + ((AsFilename == "") ? name : AsFilename));
                    break;
                default:
                    Logger.WriteLog("   Error: Could not find " + ((AsFilename == "") ? name : AsFilename));
                    break;
            }
        }
    }
}
