using System.IO;
using Newtonsoft.Json;
using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.Services
{
    public class SettingsProvider : ISettingsProvider
    {
        private readonly string _settingsFilePath;
        private AppSettings _settings;

        public SettingsProvider(string settingsFilePath)
        {
            _settingsFilePath = settingsFilePath;
        }

        public AppSettings Settings
        {
            get
            {
                if (_settings == null)
                {
                    var settingsFileContent = File.ReadAllText(_settingsFilePath);
                    _settings = JsonConvert.DeserializeObject<AppSettings>(settingsFileContent);
                }

                return _settings;
            }

            set => _settings = value;
        }
    }
}