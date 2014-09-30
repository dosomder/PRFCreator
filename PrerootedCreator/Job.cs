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
            if (!File.Exists(form.rec_textbox.Text)) //if recovery is not included
                count--;

            return count; //Don't count 'Complete'
        }

        private static Action<BackgroundWorker>[] jobs = { UnpackSystem, UnpackSystemEXT4, EditScript, AddSystem, AddExtras, AddSuperSU, AddRecovery, Complete };
        public static void Worker()
        {
            JobNum = 0;
            int free = Utility.freeSpaceMB(System.IO.Path.GetTempPath());
            if (free < 3000)
            {
                Logger.WriteLog("Error: Not enough disk space. Please make sure that atleast 3GB are free. Currently you only have " + free + "MB available");
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
            Zipping.UnzipFile(worker, form.ftf_textbox.Text, "system.sin", string.Empty, System.IO.Path.GetTempPath());

            byte[] UUID = PartitionInfo.ReadSinUUID(System.IO.Path.GetTempPath() + "\\system.sin");
            PartitionInfo.UsingUUID = (UUID != null);
            Utility.ScriptSetUUID(worker, "system", UUID);
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
            SinExtract.ExtractSin(worker, System.IO.Path.GetTempPath() + "\\system.sin", System.IO.Path.GetTempPath() + "\\system.ext4");
            File.Delete(System.IO.Path.GetTempPath() + "\\system.sin");
        }

        private static void AddSystem(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding system to zip");
            Zipping.AddToZip(worker, "flashable.zip", System.IO.Path.GetTempPath() + "\\system.ext4", "system.ext4");
            File.Delete(System.IO.Path.GetTempPath() + "\\system.ext4");
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

        private static void AddSuperSU(BackgroundWorker worker)
        {
            SetJobNum(++JobNum);
            Logger.WriteLog("Adding " + Path.GetFileName(form.su_textbox.Text));
            string superSUFile = form.su_textbox.Text;
            Zipping.AddToZip(worker, "flashable.zip", superSUFile, "SuperSU.zip", false);
        }

        private static void AddRecovery(BackgroundWorker worker)
        {
            if (!File.Exists(form.rec_textbox.Text))
                return;

            SetJobNum(++JobNum);
            string recoveryFile = form.rec_textbox.Text;
            Logger.WriteLog("Adding " + Path.GetFileName(recoveryFile));
            Zipping.AddToZip(worker, "flashable.zip", recoveryFile, "dualrecovery.zip");
        }

        private static void Complete(BackgroundWorker worker)
        {
            Logger.WriteLog("Finished\n");
        }

        private static void Cancel(BackgroundWorker worker)
        {
            Logger.WriteLog("Cancelled\n");
        }
    }
}
