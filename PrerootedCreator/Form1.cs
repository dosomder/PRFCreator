using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PRFCreator
{
    public partial class Form1 : Form
    {
        public static bool isWorking = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            version_label.Text = "v1.0";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Multiselect = false;

            Logger.form = Job.form = this;
            Settings.ReadSettings();
        }

        private void ftf_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            openFileDialog1.Filter = "FTF Files|*.ftf|All files|*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
                return;

            ftf_textbox.Text = openFileDialog1.FileName;
            openFileDialog1.FileName = string.Empty;
        }

        private void su_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            openFileDialog1.Filter = "Zip Files|*.zip|All files|*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
                return;

            su_textbox.Text = openFileDialog1.FileName;
            openFileDialog1.FileName = string.Empty;
        }

        private void versionlbl_Click(object sender, EventArgs e)
        {
            MessageBox.Show("PRFCreator " + version_label.Text + "\n\nCreated by zxz0O0\nThanks to Androxyde, [NUT], E:V:A and dotnetzip developers\n" +
                "See xda-developers.com for more informations", "PRFCreator");
        }

        private void create_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;
            if (!System.IO.File.Exists(ftf_textbox.Text))
            {
                Logger.WriteLog("Error: Please specify a valid FTF file");
                return;
            }
            if (!System.IO.File.Exists(su_textbox.Text))
            {
                Logger.WriteLog("Error: Please specify a valid SuperSU.zip");
                return;
            }
            else if (Zipping.ExistsInZip(su_textbox.Text, "updater-script"))
            {
                Logger.WriteLog("Info: No updater-script found in SuperSU zip. Are you sure it's a flashable zip?");
            }
            if (!System.IO.File.Exists(rec_textbox.Text))
            {
                Logger.WriteLog("Info: Not adding recovery");
            }
            else if (Zipping.ExistsInZip(rec_textbox.Text, "updater-script"))
            {
                Logger.WriteLog("Info: No updater-script found in Recovery zip. Are you sure it's a flashable zip?");
            }

            if (Settings.saveDialog)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Zip Files (*.zip)|*.zip";
                    if (sfd.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                        return;

                    Settings.destinationFile = sfd.FileName;
                }
            }

            Utility.WriteResourceToFile("PRFCreator.Resources.flashable.zip", Settings.destinationFile);
            if (!System.IO.File.Exists(Settings.destinationFile))
            {
                Logger.WriteLog("Error: Unable to extract flashable.zip from the exe");
                return;
            }

            Job.Worker();
        }

        public void ControlsEnabled(bool Enabled)
        {
            isWorking = !Enabled;
            if (ftf_textbox.InvokeRequired)
                ftf_textbox.Invoke(new MethodInvoker(delegate { ftf_textbox.ReadOnly = !Enabled; }));
            else
                ftf_textbox.ReadOnly = !Enabled;
            if (su_textbox.InvokeRequired)
                su_textbox.Invoke(new MethodInvoker(delegate { su_textbox.ReadOnly = !Enabled; }));
            else
                su_textbox.ReadOnly = !Enabled;
            if (rec_textbox.InvokeRequired)
                rec_textbox.Invoke(new MethodInvoker(delegate { rec_textbox.ReadOnly = !Enabled; }));
            else
                rec_textbox.ReadOnly = !Enabled;
            if(include_checklist.InvokeRequired)
                include_checklist.Invoke(new MethodInvoker(delegate { include_checklist.Enabled = Enabled; }));
            else
                include_checklist.Enabled = Enabled;
            if (create_button.InvokeRequired)
                create_button.Invoke(new MethodInvoker(delegate { create_button.Enabled = Enabled; }));
            else
                create_button.Enabled = Enabled;
            if (options_checklist.InvokeRequired)
                options_checklist.Invoke(new MethodInvoker(delegate { options_checklist.Enabled = Enabled; }));
            else
                options_checklist.Enabled = Enabled;
            if (add_extra_button.InvokeRequired)
                add_extra_button.Invoke(new MethodInvoker(delegate { add_extra_button.Enabled = Enabled; }));
            else
                add_extra_button.Enabled = Enabled;
            if (remove_extra_button.InvokeRequired)
                remove_extra_button.Invoke(new MethodInvoker(delegate { remove_extra_button.Enabled = Enabled; }));
            else
                remove_extra_button.Enabled = Enabled;
        }

        private void dr_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            openFileDialog1.Filter = "Zip Files|*.zip|All files|*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
                return;

            rec_textbox.Text = openFileDialog1.FileName;
            openFileDialog1.FileName = string.Empty;
        }

        private void add_extra_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            openFileDialog1.Filter = "Zip Files|*.zip|All files|*";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result != DialogResult.OK)
                return;

            if (!extra_listbox.Items.Contains(openFileDialog1.FileName))
                extra_listbox.Items.Add(openFileDialog1.FileName);

            openFileDialog1.FileName = string.Empty;
        }

        private void remove_extra_button_Click(object sender, EventArgs e)
        {
            if (isWorking)
                return;

            extra_listbox.Items.Remove(extra_listbox.SelectedItem);
        }
    }
}
