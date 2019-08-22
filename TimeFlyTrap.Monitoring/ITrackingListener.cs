namespace TimeFlyTrap.Monitoring
{
    public interface ITrackingListener
    {
        void OnActiveWindowInfo(OnActiveWindowInfoEvent @event);
    }
}