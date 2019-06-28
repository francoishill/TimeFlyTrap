using TimeFlyTrap.WpfApp.Domain;
using TimeFlyTrap.WpfApp.Settings;

namespace TimeFlyTrap.WpfApp.Services
{
    public class SettingsProvider : ISettingsProvider
    {
        public AppSettings Settings { get; set; }
    }
}