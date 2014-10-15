using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace PRFCreator
{
    static class Utility
    {
        public static bool JavaInstalled()
        {
            try
            {
                int exitcode = RunProcess("java.exe", "-version");
                if (exitcode != 0)
                    throw new ApplicationException("Error: Unexpected exit code: " + exitcode);
            }
            catch (Win32Exception)
            {
                return false;
            }

            return true;
        }

        public static int RunProcess(string file, string argument)
        {
            Process p = new Process();
            p.StartInfo.FileName = file;
            p.StartInfo.Arguments = argument;
            p.StartInfo.CreateNoWindow = !(p.StartInfo.UseShellExecute = false);
            p.Start();
            p.WaitForExit();
            return p.ExitCode;
        }

        public static int freeSpaceMB(string path)
        {
            string driveLetter = Path.GetPathRoot(path);
            foreach (DriveInfo dinfo in DriveInfo.GetDrives())
            {
                if (dinfo.Name.Equals(driveLetter, StringComparison.CurrentCultureIgnoreCase))
                    return (int)(dinfo.AvailableFreeSpace / 1024 / 1024);
            }

            return 0;
        }

        public static void ScriptSetUUID(BackgroundWorker worker, string old, byte[] UUID)
        {
            if (!PartitionInfo.UsingUUID)
                return;

            old = old.ToUpper() + "UUID";
            string UUIDstr = UUIDtoString(UUID);
            EditScript(worker, old, UUIDstr);
        }

        //UUID example sgdisk: F9CDF7BA-B834-A72A-F1C9-D6E0C0983896
        //UUID example sinfile: BAF7CDF9 34B8 2AA7 F1C9 D6E0C0983896
        private static string UUIDtoString(byte[] UUID)
        {
            //no idea why this part is reversed (comparing sin file and sgdisk output)
            Array.Reverse(UUID, 0, 4);
            Array.Reverse(UUID, 4, 2);
            Array.Reverse(UUID, 6, 2);
            //returns AA-5E-86-24
            string uuid = BitConverter.ToString(UUID);
            uuid = uuid.Replace("-", "");
            uuid = uuid.Insert(8, "-");
            uuid = uuid.Insert(13, "-");
            uuid = uuid.Insert(18, "-");
            uuid = uuid.Insert(23, "-");
            if (uuid.Length != 36)
                throw new Exception("Woot, spooky uuid");

            return uuid;
        }

        public static void EditScript(BackgroundWorker worker, string search, string replace)
        {
            Zipping.UnzipFile(worker, "flashable.zip", "updater-script", "META-INF/com/google/android", System.IO.Path.GetTempPath(), false);
            string content = File.ReadAllText(System.IO.Path.GetTempPath() + "\\updater-script", Encoding.ASCII);
            content = content.Replace(search, replace);
            File.WriteAllText(System.IO.Path.GetTempPath() + "\\updater-script", content, Encoding.ASCII);
            Zipping.AddToZip(worker, "flashable.zip", System.IO.Path.GetTempPath() + "\\updater-script", "META-INF/com/google/android/updater-script", false);
            File.Delete(System.IO.Path.GetTempPath() + "\\updater-script");
        }

        //http://stackoverflow.com/questions/2989400/store-files-in-c-sharp-exe-file
        public static void WriteResourceToFile(string resourceName, string fileName)
        {
            int bufferSize = 4096; // set 4KB buffer
            byte[] buffer = new byte[bufferSize];
            using (Stream input = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            using (Stream output = new FileStream(fileName, FileMode.Create))
            {
                int byteCount = input.Read(buffer, 0, bufferSize);
                while (byteCount > 0)
                {
                    output.Write(buffer, 0, byteCount);
                    byteCount = input.Read(buffer, 0, bufferSize);
                }
            }
        }

        public static string PadStr(string str, string pad, int length)
        {
            while (str.Length < length)
            {
                if ((length - str.Length) % 2 == 0)
                    str = pad + str + pad;
                else
                    str = pad + str;
            }
            return str;
        }

        //Convert big endian to little endian
        public static int ReadIntBigEndian(BinaryReader br)
        {
            byte[] baInt = br.ReadBytes(4);
            Array.Reverse(baInt);
            return BitConverter.ToInt32(baInt, 0);
        }

        public static long ReadLongBigEndian(BinaryReader br)
        {
            byte[] baLong = br.ReadBytes(8);
            Array.Reverse(baLong);
            return BitConverter.ToInt64(baLong, 0);
        }

        public static bool byteArrayCompare(byte[] arr1, byte[] arr2)
        {
            if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }
    }
}
