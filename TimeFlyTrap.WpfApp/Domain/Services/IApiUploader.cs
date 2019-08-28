using TimeFlyTrap.Monitoring;

namespace TimeFlyTrap.WpfApp.Domain.Services
{
    public interface IApiUploader
    {
        void OnActiveWindowInfo(OnActiveWindowInfoEvent @event);

        void StopUploading();
    }
}