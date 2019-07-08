using System.Collections.Generic;

namespace TimeFlyTrap.Monitoring
{
    public interface IActiveWindowsTracker
    {
        void StartTicker(ITrackingListener trackingListener);
        Dictionary<string, WindowTimes> GetReport(bool clearList);
    }
}