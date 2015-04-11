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

        public static void AddExtraFlashable(BackgroundWorker worker, string filename, string ftffile)
        {
            Logger.WriteLog("Adding flashable zip: " + Path.GetFileName(filename));
            string fixedname = Path.GetFileName(filename).Replace(' ', '_');

            string cmd = "\n# " + fixedname + "\n" +
                "if\n" +
                "\tpackage_extract_file(\"" + fixedname + "\", \"/tmp/" + fixedname + "\") == \"t\"\n" +
                "then\n" +
                "\trun_program(\"/tmp/busybox\", \"mkdir\", \"/tmp/" + Path.GetFileNameWithoutExtension(fixedname) + "_extracted" + "\");\n" +
                "\trun_program(\"/tmp/busybox\", \"unzip\", \"-d\", \"/tmp/" + Path.GetFileNameWithoutExtension(fixedname) + "_extracted" + "\", \"/tmp/" + fixedname + "\");\n" +
                "\tset_perm(0, 0, 0755, \"/tmp/" + Path.GetFileNameWithoutExtension(fixedname) + "_extracted" + "/META-INF/com/google/android/update-binary\");\n" +
                "\trun_program(\"/tmp/" + Path.GetFileNameWithoutExtension(fixedname) + "_extracted" + "/META-INF/com/google/android/update-binary\", file_getprop(\"/tmp/prfargs\", \"version\"), file_getprop(\"/tmp/prfargs\", \"outfile\"), \"/tmp/" + fixedname + "\");\n" +
                "\tdelete_recursive(\"/tmp/" + Path.GetFileNameWithoutExtension(fixedname) + "_extracted" + "\");\n" +
                "\tdelete(\"/tmp/" + fixedname + "\");\n" +
                "endif;\n" +
                "#InsertExtra\n";
            Utility.EditScript(worker, "#InsertExtra", cmd);
            Zipping.AddToZip(worker, "flashable.zip", filename, fixedname, false);
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
                    ExtractAndAdd(worker, modem, string.Empty, ftffile, "amss_fsg");
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
            if (File.Exists(Path.Combine(Path.GetTempPath(), name + ".sin")))
            {
                Logger.WriteLog("   " + name);
                SinExtract.ExtractSin(worker, Path.Combine(Path.GetTempPath(), name + ".sin"), Path.Combine(Path.GetTempPath(), name + extension), false);

                if (PartitionInfo.UsingUUID)
                {
                    byte[] UUID = PartitionInfo.ReadSinUUID(Path.Combine(Path.GetTempPath(), name + ".sin"));
                    Utility.ScriptSetUUID(worker, (AsFilename == "" ? name : AsFilename), UUID);
                }

                File.Delete(Path.Combine(Path.GetTempPath(), name + ".sin"));
                Zipping.AddToZip(worker, "flashable.zip", Path.Combine(Path.GetTempPath(), name + extension), (AsFilename == "" ? name : AsFilename) + extension, false);
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
