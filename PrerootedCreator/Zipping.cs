using System;
using System.Collections.Generic;
using System.Text;
using Ionic.Zip;
using System.IO;
using System.ComponentModel;

namespace PRFCreator
{
    static class Zipping
    {
        public static bool ExistsInZip(string zipfile, string file)
        {
            try
            {
                using (ZipFile zip = new ZipFile(zipfile))
                {
                    foreach (ZipEntry ze in zip.Entries)
                        if (ze.FileName == file)
                            return true;
                }
            }
            catch (Ionic.Zip.BadReadException)
            {
                Logger.WriteLog("Error: The zip file " + zipfile + " seems corrupt");
            }
            return false;
        }

        //Returns true on success and false if there is no entry called oldfile
        public static bool RenameInZip(string zipfile, string oldfile, string newfile)
        {
            using (ZipFile zip = new ZipFile(zipfile))
            {
                Utility.SetZipTempFolder(zip);
                ICollection<ZipEntry> zipEntries = zip.SelectEntries("name = '" + oldfile + "'");
                if (zipEntries.Count == 0)
                    return false;

                foreach (ZipEntry ze in zipEntries)
                {
                    ze.FileName = newfile;
                    break;
                }
                zip.Save();
            }
            return true;
        }

        public static string ZipGetFullname(string zipfile, string pattern)
        {
            using (ZipFile zip = new ZipFile(zipfile))
            {
                ICollection<ZipEntry> zes = zip.SelectEntries("name = '" + pattern + "'");
                foreach (ZipEntry ze in zes)
                    return ze.FileName;
            }
            return string.Empty;
        }

        /// <summary> 
        /// Unzip a file.
        /// </summary> 
        /// <param name="worker">
        /// Backgroundworker to report progress to.
        /// </param> 
        /// <param name="zipfile">
        /// A valid zip file.
        /// </param> 
        /// <param name="file">
        /// File to unzip inside the zipfile.
        /// </param> 
        /// <param name="path">
        /// Path to the file inside the zipfile.
        /// </param>
        /// <param name="destination">
        /// Destination file name.
        /// </param> 
        /// <param name="showProgress">
        /// Report progress to the background worker.
        /// </param> 
        /// <returns> 
        /// Returns true on success and false if the zip file was not valid.
        /// </returns> 
        public static bool UnzipFile(BackgroundWorker worker, string zipfile, string file, string path, string destination, bool showProgress = true)
        {
            if (File.Exists(destination))
                throw new ArgumentException("UnzipFile: Argument destination is expected to be a folder, not a file");
            if (!Directory.Exists(destination))
                throw new DirectoryNotFoundException("Destination directory " + destination + " does not exist");

            //Ionic creates a tmp file and throws an exception if it already exists
            File.Delete(Path.Combine(destination, file + ".tmp"));
            try
            {
                using (ZipFile zip = ZipFile.Read(zipfile))
                {
                    if (showProgress)
                        zip.ExtractProgress += (o, e) =>
                        {
                            if (e.EventType == ZipProgressEventType.Extracting_EntryBytesWritten)
                                worker.ReportProgress((int)((float)e.BytesTransferred / e.TotalBytesToTransfer * 100));
                        };

                    zip.FlattenFoldersOnExtract = true;
                    ICollection<ZipEntry> zes = zip.SelectEntries("name = '" + file + "'", path);
                    foreach (ZipEntry ze in zes)
                    {
                        ze.Extract(destination, ExtractExistingFileAction.OverwriteSilently);
                        //just extract one file
                        break;
                    }
                }
            }
            catch (Ionic.Zip.BadCrcException)
            {
                Logger.WriteLog("Error unzipping the file: " + zipfile + "\nPlease make sure it's a valid zip file or try redownloading the file");
                System.Windows.Forms.MessageBox.Show("PRFCreator", "Error unzipping the file: " + zipfile + "\n" +
                    "Please make sure it's a valid zip file or try redownloading the file", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        public static void AddToZip(BackgroundWorker worker, string zipfile, string FileToAdd, string AsFilename = "", bool showProgress = true)
        {
            if (!File.Exists(zipfile))
                throw new FileNotFoundException("Zipfile " + zipfile + " does not exist");

            bool exists = ExistsInZip(zipfile, AsFilename == "" ? FileToAdd : AsFilename);
            using (ZipFile zip = new ZipFile(zipfile))
            {
                Utility.SetZipTempFolder(zip);

                if (exists)
                    zip.RemoveEntry(AsFilename == "" ? FileToAdd : AsFilename);
                ZipEntry ze = zip.AddFile(FileToAdd, "");
                if (!string.IsNullOrEmpty(AsFilename))
                    ze.FileName = AsFilename;

                if (showProgress)
                    zip.SaveProgress += (o, e) =>
                    {
                        if (e.EventType == ZipProgressEventType.Saving_EntryBytesRead && e.CurrentEntry.FileName == (AsFilename == "" ? FileToAdd : AsFilename))
                            worker.ReportProgress((int)((float)e.BytesTransferred / e.TotalBytesToTransfer * 100));
                    };
                zip.Save();
            }
        }
    }
}
