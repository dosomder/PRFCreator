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
            version_label.Text = "v0.7";
            openFileDialog1.FileName = string.Empty;
            openFileDialog1.Multiselect = false;

            Logger.form = Job.form = this;
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
            Utility.WriteResourceToFile("PRFCreator.Resources.flashable.zip", "flashable.zip");
            if (!System.IO.File.Exists("flashable.zip"))
            {
                Logger.WriteLog("Error: Can not find flashable.zip in the same directory");
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
    }
}
