using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PRFCreator
{
    class Logger
    {
        private static string GetTimeDate()
        {
            string timedate = string.Empty;
            timedate = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss");

            return timedate;
        }

        private static void CleanLog()
        {
            int ldel = Form1.form.status_textbox.Lines.Length - 60;
            if (ldel > 0)
            {
                string txt = Form1.form.status_textbox.Text;
                while (ldel-- > 0)
                    txt = txt.Substring(txt.IndexOf('\n') + 1);

                Form1.form.status_textbox.Text = txt;
            }
        }

        public static void WriteLog(string str)
        {
            Utility.InvokeIfNecessary(Form1.form.status_textbox, new MethodInvoker(CleanLog));
            str = GetTimeDate() + " - " + str + "\n";

            Utility.InvokeIfNecessary(Form1.form.status_textbox, new MethodInvoker(delegate 
                { 
                    Form1.form.status_textbox.AppendText(str);
                    Form1.form.status_textbox.ScrollToCaret();
                }));
        }
    }
}
