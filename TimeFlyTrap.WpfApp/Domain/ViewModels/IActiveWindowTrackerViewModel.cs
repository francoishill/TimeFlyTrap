using Microsoft.Extensions.Logging;

namespace TimeFlyTrap.WpfApp.Domain.ViewModels
{
    public interface IActiveWindowTrackerViewModel : ILogger
    {
        void StartTracking();
    }
}