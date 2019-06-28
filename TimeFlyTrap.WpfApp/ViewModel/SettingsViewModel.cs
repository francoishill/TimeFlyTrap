using System;
using System.IO;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TimeFlyTrap.WpfApp.Domain;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Settings;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class SettingsViewModel : ViewModelBase, ISettingsViewModel
    {
        private readonly ILogger _logger;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IAppFilePathProvider _appFilePathProvider;

        private string _settingsJson;

        public SettingsViewModel(
            ILogger logger,
            ISettingsProvider settingsProvider,
            IAppFilePathProvider appFilePathProvider)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _appFilePathProvider = appFilePathProvider;

            LoadSettings();
        }

        public string SettingsJson
        {
            get => _settingsJson;

            set
            {
                try
                {
                    var settings = JsonConvert.DeserializeObject<AppSettings>(value);
                    _settingsJson = value;
                    _settingsProvider.Settings = settings;
                    SaveSettings(settings);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Invalid settings JSON: {value}");
                }
            }
        }

        private void LoadSettings()
        {
            var settingsFilePath = _appFilePathProvider.SettingsFilePath;

            if (File.Exists(settingsFilePath))
            {
                var settingsJson = File.ReadAllText(settingsFilePath);

                try
                {
                    var settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);
                    _settingsProvider.Settings = settings;
                    _settingsJson = settingsJson;
                    return;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Using default settings, invalid settings file content in {settingsFilePath}, content: {_settingsJson}");
                }
            }

            _settingsProvider.Settings = new AppSettings();
        }

        private void SaveSettings(AppSettings appSettings)
        {
            var settingsFilePath = _appFilePathProvider.SettingsFilePath;

            try
            {
                var json = JsonConvert.SerializeObject(appSettings);
                File.WriteAllText(settingsFilePath, json);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to save settings");
            }
        }
    }
}