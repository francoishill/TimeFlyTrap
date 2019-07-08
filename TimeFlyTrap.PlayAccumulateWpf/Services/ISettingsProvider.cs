using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.Services
{
    public interface ISettingsProvider
    {
        AppSettings Settings { get; set; }
    }
}