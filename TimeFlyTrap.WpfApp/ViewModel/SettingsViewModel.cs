using System;
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
        private readonly IAppFileSystem _appFileSystem;

        private string _settingsJson;

        public SettingsViewModel(
            ILogger logger,
            ISettingsProvider settingsProvider,
            IAppFileSystem appFileSystem)
        {
            _logger = logger;
            _settingsProvider = settingsProvider;
            _appFileSystem = appFileSystem;

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
            if (_appFileSystem.ReadSettingsFile(out var settingsJson))
            {
                try
                {
                    var settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);
                    _settingsProvider.Settings = settings;
                    _settingsJson = settingsJson;
                    return;
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, $"Using default settings, invalid settings file content: {_settingsJson}");
                }
            }

            _settingsProvider.Settings = new AppSettings();
        }

        private void SaveSettings(AppSettings appSettings)
        {
            try
            {
                var json = JsonConvert.SerializeObject(appSettings);
                _appFileSystem.SaveSettings(json);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to save settings");
            }
        }
    }
}