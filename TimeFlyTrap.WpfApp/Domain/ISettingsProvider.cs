using TimeFlyTrap.WpfApp.Settings;

namespace TimeFlyTrap.WpfApp.Domain
{
    public interface ISettingsProvider
    {
        AppSettings Settings { get; set; }
    }
}