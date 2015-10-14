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

        public static void AddAPKFile(BackgroundWorker worker, string filename, string type)
        {
            Logger.WriteLog("Adding APK: " + Path.GetFileName(filename));
            if (type == "App (System)")
                Zipping.AddToZip(worker, Settings.destinationFile, filename, "system/app/" + Path.GetFileName(filename), false);
            else
                Zipping.AddToZip(worker, Settings.destinationFile, filename, "data/app/" + Path.GetFileName(filename), false);
        }

        public static void AddExtraFlashable(BackgroundWorker worker, string filename)
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
            Zipping.AddToZip(worker, Settings.destinationFile, filename, fixedname, false);
        }

        private static string GetKernelFilename(string ftffile)
        {
            string[] names = { "kernel", "boot" };
            foreach (string name in names)
            {
                if (Zipping.ExistsInZip(ftffile, name + ".sin"))
                    return name;
            }

            //if nothing exists, return kernel anyway so the error message makes sense
            return "kernel";
        }

        private static void AddKernel(BackgroundWorker worker, string ftffile)
        {
            if (PartitionInfo.ScriptMode == PartitionInfo.Mode.Sinflash)
            {
                ExtractAndAddSin(worker, GetKernelFilename(ftffile), ftffile, "boot");
                ExtractAndAddSin(worker, "rpm", ftffile);
            }
            else
            {
                ExtractAndAdd(worker, GetKernelFilename(ftffile), ".elf", ftffile, "boot");
                ExtractAndAdd(worker, "rpm", ".elf", ftffile);
            }

            Utility.EditConfig(worker, "KERNEL", "1");
        }

        private static void AddFOTAKernel(BackgroundWorker worker, string ftffile)
        {
            if (PartitionInfo.ScriptMode == PartitionInfo.Mode.Sinflash)
                ExtractAndAddSin(worker, "fotakernel", ftffile);
            else
                ExtractAndAdd(worker, "fotakernel", ".elf", ftffile);

            Utility.EditConfig(worker, "FOTAKERNEL", "1");
        }

        private static void AddLTALabel(BackgroundWorker worker, string ftffile)
        {
            string ltalname = Zipping.ZipGetFullname(ftffile, "elabel*.sin");
            if (string.IsNullOrEmpty(ltalname))
            {
                Logger.WriteLog("   Error: Could not find LTALabel in FTF");
                return;
            }

            if (PartitionInfo.ScriptMode == PartitionInfo.Mode.Sinflash)
                ExtractAndAddSin(worker, Path.GetFileNameWithoutExtension(ltalname), ftffile, "ltalabel");
            else
                ExtractAndAdd(worker, Path.GetFileNameWithoutExtension(ltalname), ".ext4", ftffile, "ltalabel");

            Utility.EditConfig(worker, "LTALABEL", "1");
        }

        private static string GetModemFilename(string ftffile)
        {
            string[] mdms = { "amss_fsg", "amss_fs_3", "modem" };
            foreach (string mdm in mdms)
            {
                if (Zipping.ExistsInZip(ftffile, mdm + ".sin"))
                    return mdm;
            }

            return "modem";
        }

        private static void AddModem(BackgroundWorker worker, string ftffile)
        {
            if (PartitionInfo.ScriptMode == PartitionInfo.Mode.Sinflash)
            {
                ExtractAndAddSin(worker, GetModemFilename(ftffile), ftffile, "amss_fsg");
                ExtractAndAddSin(worker, "amss_fs_1", ftffile);
                ExtractAndAddSin(worker, "amss_fs_2", ftffile);
            }
            else
            {
                ExtractAndAdd(worker, GetModemFilename(ftffile), string.Empty, ftffile, "amss_fsg");
                ExtractAndAdd(worker, "amss_fs_1", string.Empty, ftffile);
                ExtractAndAdd(worker, "amss_fs_2", string.Empty, ftffile);
            }

            Utility.EditConfig(worker, "MODEM", "1");
        }

        private static void ExtractAndAdd(BackgroundWorker worker, string name, string extension, string ftffile, string AsFilename = "")
        {
            if (Zipping.ExistsInZip(ftffile, name + ".sin") == false)
            {
                OnError(name, AsFilename);
                return;
            }

            Zipping.UnzipFile(worker, ftffile, name + ".sin", string.Empty, Utility.GetTempPath(), false);
            if (File.Exists(Path.Combine(Utility.GetTempPath(), name + ".sin")))
            {
                Logger.WriteLog("   " + name);
                SinExtract.ExtractSin(worker, Path.Combine(Utility.GetTempPath(), name + ".sin"), Path.Combine(Utility.GetTempPath(), name + extension), false);

                if (PartitionInfo.ScriptMode == PartitionInfo.Mode.LegacyUUID)
                {
                    byte[] UUID = PartitionInfo.ReadSinUUID(Path.Combine(Utility.GetTempPath(), name + ".sin"));
                    Utility.ScriptSetUUID(worker, (AsFilename == "" ? name : AsFilename), UUID);
                }

                File.Delete(Path.Combine(Utility.GetTempPath(), name + ".sin"));
                Zipping.AddToZip(worker, Settings.destinationFile, Path.Combine(Utility.GetTempPath(), name + extension), (AsFilename == "" ? name : AsFilename) + extension, false);
                File.Delete(Path.Combine(Utility.GetTempPath(), name + extension));
            }
        }

        private static void ExtractAndAddSin(BackgroundWorker worker, string name, string ftffile, string AsFilename = "")
        {
            if (Zipping.ExistsInZip(ftffile, name + ".sin") == false)
            {
                OnError(name, AsFilename);
                return;
            }

            Zipping.UnzipFile(worker, ftffile, name + ".sin", string.Empty, Utility.GetTempPath(), false);
            if (File.Exists(Path.Combine(Utility.GetTempPath(), name + ".sin")))
            {
                Logger.WriteLog("   " + name);
                Zipping.AddToZip(worker, Settings.destinationFile, Path.Combine(Utility.GetTempPath(), name + ".sin"), (AsFilename == "" ? name : AsFilename) + ".sin", false, Ionic.Zlib.CompressionLevel.None);
                File.Delete(Path.Combine(Utility.GetTempPath(), name + ".sin"));
            }
        }

        private static void OnError(string name, string AsFilename = "")
        {
            switch (name)
            {
                case "rpm":
                    //rpm seems to be missing in older firmwares, so we don't display it as error
                    //in newer firmwares, rpm is moved to boot files
                    //Logger.WriteLog("   Info: Could not find " + ((AsFilename == "") ? name : AsFilename));
                    break;
                default:
                    Logger.WriteLog("   Error: Could not find " + ((AsFilename == "") ? name : AsFilename));
                    break;
            }
        }
    }
}
