using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;

namespace PRFCreator
{
    class Job
    {
        public static Form1 form;

        private static int JobNum = 0;
        private static void SetJobNum(int num)
        {
            if (form.jobnum_label.InvokeRequired)
                form.jobnum_label.Invoke(new MethodInvoker(delegate { form.jobnum_label.Text = num + "/" + GetJobCount(); }));
            else
                form.jobnum_label.Text = num + "/" + GetJobCount();
        }

        private static int GetJobCount()
        {
            int count = jobs.Length - 1; //don't count 'Complete'
            if (form.include_checklist.CheckedItems.Count < 1) //if there are no extra files
                count--;
            if (!form.options_checklist.CheckedItems.Contains("Sign zip"))
                count--;
            if (!File.Exists(form.rec_textbox.Text)) //if recovery is not included
                count--;
            if (form.extra_dataGridView.Rows.Count < 1) //no additional zip files
                count--;

            return count;
        }

        private static Action<BackgroundWorker>[] legacyjobs = { UnpackSystem, UnpackSystemEXT4, EditScript, AddExtras, AddSuperSU, AddRecovery, AddExtraFlashable, AddSystemEXT4, SignZip, Complete };
        private static Action<BackgroundWorker>[] newjobs = { UnpackSystem, EditScript, UnpackPartitionImage, AddParititonImage, AddExtras, AddSuperSU, AddRecovery, AddExtraFlashable, AddSystem, SignZip, Complete };
        private static Action<BackgroundWorker>[] jobs = newjobs;
        public static void Worker()
        {
            JobNum = 0;
            int free = Utility.freeSpaceMB(Utility.GetTempPath());
            if (free < 4096)
            {
                Logger.WriteLog("Error: Not enough disk space. Please make sure that you have atleast 4GB free space on drive " + Path.GetPathRoot(Utility.GetTempPath())
                    + ". Currently you only have " + free + "MB available");
                return;
            }
            if (!Zipping.ExistsInZip(form.ftf_textbox.Text, "system.sin"))
            {
                Logger.WriteLog("Error: system.sin does not exist in file " + form.ftf_textbox.Text);
                return;
            }
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += (o, _e) =>
                {
                    try
                    {
                        form.ControlsEnabled(false);
                        if (form.options_checklist.CheckedItems.Contains("Legacy mode"))
                            jobs = legacyjobs;
                        else
                            jobs = newjobs;
                        foreach (Action<BackgroundWorker> action in jobs)
                        {
                            if (worker.CancellationPending)
                            {
                                Cancel(worker);
                                _e.Cancel = true;
                                break;
                            }
                            action(worker);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(e.Message);
                        //Logger.WriteLog(e.StackTrace);
                    }
                };
            worker.ProgressChanged += (o, _e) =>
                {
                    form.progressBar.Value = _e.ProgressPercentage;
                };
            worker.RunWorkerCompleted += (o, _e) =>
                {
                    form.ControlsEnabled(true);
                };
            worker.RunWorkerAsync();
        }

        private static void UnpackSystem(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Extracting system.sin from " + System.IO.Path.GetFileName(form.ftf_textbox.Text));
            if (!Zipping.UnzipFile(worker, form.ftf_textbox.Text, "system.sin", string.Empty, Utility.GetTempPath()))
            {
                worker.CancelAsync();
                return;
            }

            byte[] UUID = PartitionInfo.ReadSinUUID(Path.Combine(Utility.GetTempPath(), "system.sin"));
            //PartitionInfo.ScriptMode = (UUID != null) ? PartitionInfo.Mode.LegacyUUID : PartitionInfo.Mode.Legacy;
            if (!form.options_checklist.CheckedItems.Contains("Legacy mode"))
                PartitionInfo.ScriptMode = PartitionInfo.Mode.Sinflash;
            else
                PartitionInfo.ScriptMode = (UUID != null) ? PartitionInfo.Mode.LegacyUUID : PartitionInfo.Mode.Legacy;
            Utility.ScriptSetUUID(worker, "system", UUID);
        }

        private static void UnpackPartitionImage(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Extracting partition-image.sin from " + System.IO.Path.GetFileName(form.ftf_textbox.Text));
            if (!Zipping.UnzipFile(worker, form.ftf_textbox.Text, "partition-image.sin", string.Empty, Utility.GetTempPath()))
            {
                if (Zipping.UnzipFile(worker, form.ftf_textbox.Text, "partition.sin", string.Empty, Utility.GetTempPath()))
                    File.Move(Path.Combine(Utility.GetTempPath(), "partition.sin"), Path.Combine(Utility.GetTempPath(), "partition-image.sin"));
                else
                {
                    Logger.WriteLog("Error extracting partition-image.sin from ftf. Please try Legacy Mode");
                    worker.CancelAsync();
                    return;
                }
            }
        }

        private static void EditScript(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding info to flashable script");
            string fw = Utility.PadStr(Path.GetFileNameWithoutExtension(form.ftf_textbox.Text), " ", 41);
            Utility.EditScript(worker, "INSERT FIRMWARE HERE", fw);
        }

        private static void UnpackSystemEXT4(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            SinExtract.ExtractSin(worker, Path.Combine(Utility.GetTempPath(), "system.sin"), Path.Combine(Utility.GetTempPath(), "system.ext4"));
            File.Delete(Path.Combine(Utility.GetTempPath(), "system.sin"));
        }

        private static void AddSystemEXT4(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding system to zip");
            Zipping.AddToZip(worker, Settings.destinationFile, Path.Combine(Utility.GetTempPath(), "system.ext4"), "system.ext4");
            File.Delete(Path.Combine(Utility.GetTempPath(), "system.ext4"));
        }

        private static void AddSystem(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding system to zip");
            Zipping.AddToZip(worker, Settings.destinationFile, Path.Combine(Utility.GetTempPath(), "system.sin"), "system.sin", true);
            File.Delete(Path.Combine(Utility.GetTempPath(), "system.sin"));
        }

        private static void AddParititonImage(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding partition-image to zip");
            Zipping.AddToZip(worker, Settings.destinationFile, Path.Combine(Utility.GetTempPath(), "partition-image.sin"), "partition-image.sin");
            File.Delete(Path.Combine(Utility.GetTempPath(), "partition-image.sin"));
        }

        private static void AddExtras(BackgroundWorker worker)
        {
            if (form.include_checklist.CheckedItems.Count < 1)
                return;

            Logger.WriteLog("Adding extra files");
            SetJobNum(++JobNum);
            foreach (string item in form.include_checklist.CheckedItems)
                ExtraFiles.AddExtraFiles(worker, item.ToLower(), form.ftf_textbox.Text);
        }

        private static void AddExtraFlashable(BackgroundWorker worker)
        {
            if(form.extra_dataGridView.Rows.Count < 1)
                return;

            SetJobNum(++JobNum);
            for (int i = 0; i < form.extra_dataGridView.Rows.Count; i++)
            {
                string type = form.extra_dataGridView["GridViewType", i].Value.ToString();
                string name = form.extra_dataGridView["GridViewName", i].Value.ToString();
                if (type == "Flashable zip")
                    ExtraFiles.AddExtraFlashable(worker, name);
                else
                    ExtraFiles.AddAPKFile(worker, name, type);
            }
        }

        private static void AddSuperSU(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding " + Path.GetFileName(form.su_textbox.Text));
            string superSUFile = form.su_textbox.Text;
            Zipping.AddToZip(worker, Settings.destinationFile, superSUFile, "SuperSU.zip", false);
        }

        private static void AddRecovery(BackgroundWorker worker)
        {
            if (!File.Exists(form.rec_textbox.Text))
                return;

            SetJobNum(++JobNum);
            string recoveryFile = form.rec_textbox.Text;
            Logger.WriteLog("Adding " + Path.GetFileName(recoveryFile));
            Zipping.AddToZip(worker, Settings.destinationFile, recoveryFile, "dualrecovery.zip");
        }

        //~ doubles the process time
        private static void SignZip(BackgroundWorker worker)
        {
            if (!form.options_checklist.CheckedItems.Contains("Sign zip"))
                return;

            SetJobNum(++JobNum);
            if (!Utility.JavaInstalled())
            {
                Logger.WriteLog("Error: Could not execute Java. Is it installed?");
                return;
            }
            if (!File.Exists("signapk.jar"))
            {
                Logger.WriteLog("Error: signapk.jar file not found");
                return;
            }

            Utility.WriteResourceToFile("PRFCreator.Resources.testkey.pk8", "testkey.pk8");
            Utility.WriteResourceToFile("PRFCreator.Resources.testkey.x509.pem", "testkey.x509.pem");

            Logger.WriteLog("Signing zip file");
            if (Utility.RunProcess("java", "-Xmx1024m -jar signapk.jar -w testkey.x509.pem testkey.pk8 flashable.zip flashable-prerooted-signed.zip") == 0)
                File.Delete(Settings.destinationFile);
            else
                Logger.WriteLog("Error: Could not sign zip");

            File.Delete("testkey.pk8");
            File.Delete("testkey.x509.pem");
        }

        private static void Complete(BackgroundWorker worker)
        {
            File.Delete("flashable-prerooted.zip");
            if (File.Exists(Settings.destinationFile) && Settings.destinationFile == "flashable.zip")
                File.Move(Settings.destinationFile, "flashable-prerooted.zip");

            Logger.WriteLog("Finished\n");
        }

        private static void Cancel(BackgroundWorker worker)
        {
            Logger.WriteLog("Cancelled\n");
        }
    }
}
