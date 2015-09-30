using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Text;
using System.IO;

namespace PRFCreator
{
    class Settings
    {
        private const string SettingFile = "PRFCreator.xml";
        public static string templocation = null;
        public static bool saveDialog = false;
        public static string destinationFile = "flashable-prerooted.zip";

        public static void ReadSettings()
        {
            string temp = ReadSetting("templocation");
            if (!string.IsNullOrEmpty(temp))
            {
                if (!Directory.Exists(temp))
                    Logger.WriteLog("Error reading config file: Directory " + temp + " does not exist.");
                else
                    templocation = temp;
            }

            if (!bool.TryParse(ReadSetting("saveDialog"), out saveDialog))
                Logger.WriteLog("Error reading config file: Could not parse saveDialog value");
        }

        public static string ReadSetting(string element)
        {
            try
            {
                if (!File.Exists(SettingFile))
                    GenerateSettings();

                XDocument xdoc = XDocument.Load(SettingFile);
                return xdoc.Element("PRFCreator").Element(element).Value;
            }
            catch (Exception e)
            {
                Logger.WriteLog("Error reading config file: " + e.Message);
                return string.Empty;
            }
        }

        public static void SetSetting(string element, string value)
        {
            if (!File.Exists(SettingFile))
                GenerateSettings();
            XDocument xdoc = XDocument.Load(SettingFile);
            XElement xe = xdoc.Element("PRFCreator").Element(element);
            if (xe == null)
                xdoc.Element("PRFCreator").Add(new XElement(element, value));
            else
                xe.Value = value;

            xdoc.Save(SettingFile);
        }

        private static void GenerateSettings()
        {
            string preset = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
@"<PRFCreator>
    <!-- folder to use as a temp file location, empty to use %tmp% -->
    <templocation></templocation>

    <!-- display a dialog to let the user choose the destination file and path -->
    <saveDialog>False</saveDialog>
</PRFCreator>";
            File.WriteAllText(SettingFile, preset);
        }
    }
}
