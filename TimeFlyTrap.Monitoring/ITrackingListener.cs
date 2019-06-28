namespace TimeFlyTrap.Monitoring
{
    public interface ITrackingListener
    {
        void OnLastInfo(OnLastInfoEvent @event);
        void OnActiveWindowInfo(OnActiveWindowInfoEvent @event);
    }
}