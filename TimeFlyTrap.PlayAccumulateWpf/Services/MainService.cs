using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.Services
{
    public class MainService : IMainService
    {
        public IEnumerable<WindowTimes> LoadWindowTimes()
        {
            var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeFlyTrap");

            var jsonFiles = Directory
                .GetFiles(dir, "*.json", SearchOption.AllDirectories)
                .Where(file => !file.EndsWith("Settings.json", StringComparison.OrdinalIgnoreCase));

            var windowTimes = new List<WindowTimes>();
            foreach (var jsonFile in jsonFiles)
            {
                var fileContent = File.ReadAllText(jsonFile);
                var entries = JsonConvert.DeserializeObject<WindowTimes[]>(fileContent);
                windowTimes.AddRange(entries);
            }

            return windowTimes;
        }
    }
}