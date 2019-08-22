using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TimeFlyTrap.Common;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class AppFileSystem : IAppFileSystem
    {
        public string LocalAppData => AppStateHelper.ApplicationLocalDataDirectory;

        private string SettingsFilePath => Path.Combine(LocalAppData, "Settings.json");

        public bool ReadSettingsFile(out string settingsContent)
        {
            if (!File.Exists(SettingsFilePath))
            {
                settingsContent = null;
                return false;
            }

            settingsContent = File.ReadAllText(SettingsFilePath);
            return true;
        }

        public void SaveSettings(string settingsContent)
        {
            File.WriteAllText(SettingsFilePath, settingsContent);
        }

        public void WriteReport(Dictionary<string, WindowTimes> reportList)
        {
            var dir = Path.Combine(LocalAppData, $"{DateTime.Now:yyyy-MM-dd}");

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var filePathWithoutExtension = Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd HH_mm_ss}");
            var jsonFilepath = filePathWithoutExtension + ".json";
            var htmlFilepath = filePathWithoutExtension + ".html";

            if (reportList == null || reportList.Count == 0)
            {
                throw new Exception("There are no reports to save");
            }

            if (File.Exists(jsonFilepath))
            {
                File.Delete(jsonFilepath);
            }

            File.WriteAllText(jsonFilepath, JsonConvert.SerializeObject(reportList.Values.ToList()));

            var htmlText = WindowTimes.GenerateHtml(reportList);

            File.WriteAllText(htmlFilepath, htmlText);

            Process.Start("explorer", "/select,\"" + htmlFilepath + "\"");
        }
    }
}