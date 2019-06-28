using System.Collections.Generic;

namespace TimeFlyTrap.Monitoring
{
    public interface IActiveWindowsTracker
    {
        void StartTicker(ITrackingListener trackingListener);
        bool GetReport(out Dictionary<string, WindowTimes> activeWindowsList);
    }
}