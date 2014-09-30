using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PRFCreator
{
    class Logger
    {
        /*private static Form1 form;
        public Logger(Form1 _form)
        {
            form = _form;
        }*/
        public static Form1 form;

        private static string GetTimeDate()
        {
            string timedate = string.Empty;
            timedate = DateTime.Now.ToString("dd/MM/yyyy") + " " + DateTime.Now.ToString("HH:mm:ss");

            return timedate;
        }

        private static void CleanLog()
        {
            if (form.status_textbox.InvokeRequired)
                form.status_textbox.Invoke(new MethodInvoker(delegate { if (form.status_textbox.Lines.Length > 30) form.status_textbox.Text = string.Empty; }));
            else if (form.status_textbox.Lines.Length > 30)
                form.status_textbox.Text = string.Empty;
        }

        public static void WriteLog(string str)
        {
            CleanLog();
            str = GetTimeDate() + " - " + str + "\n";

            if (form.status_textbox.InvokeRequired)
            {
                form.status_textbox.Invoke(new MethodInvoker(delegate { form.status_textbox.AppendText(str); form.status_textbox.ScrollToCaret(); }));
            }
            else
            {
                form.status_textbox.AppendText(str);
                form.status_textbox.ScrollToCaret();
            }
        }
    }
}
