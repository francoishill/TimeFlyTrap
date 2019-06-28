namespace TimeFlyTrap.Monitoring
{
    public interface IActiveWindowsTracker
    {
        void StartTicker(ITrackingListener trackingListener);
    }
}