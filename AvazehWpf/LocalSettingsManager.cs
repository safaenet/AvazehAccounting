using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AvazehWpf
{
    public static class LocalSettingsManager
    {
        private readonly static string SettingsFileName = AppDomain.CurrentDomain.BaseDirectory + "appsettings.xml";
        private static void CheckSettingsFile()
        {
            if (!File.Exists(SettingsFileName))
                WriteSettingsFileToDisk(new LocalSettingsModel());
        }

        private static void WriteSettingsFileToDisk(LocalSettingsModel settings)
        {
            if (settings == null) settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            StringWriter stringWriter = new();
            xmlSerializer.Serialize(stringWriter, settings);
            File.WriteAllText(SettingsFileName, stringWriter.ToString());
        }
        public static LocalSettingsModel LoadAllSettings()
        {
            CheckSettingsFile();
            LocalSettingsModel settings = new();
            XmlSerializer xmlSerializer = new(settings.GetType());
            string xmlString = File.ReadAllText(SettingsFileName);
            StringReader stringReader = new StringReader(xmlString);
            settings = xmlSerializer.Deserialize(stringReader) as LocalSettingsModel;
            if (settings == null) settings = new();
            return settings;
        }
        public static bool SaveAllSettings(LocalSettingsModel settings)
        {
            if (settings == null) return false;
            WriteSettingsFileToDisk(settings);
            return true;
        }
    }
}