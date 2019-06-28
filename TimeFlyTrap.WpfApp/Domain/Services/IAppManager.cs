namespace TimeFlyTrap.WpfApp.Domain.Services
{
    public interface IAppManager
    {
        void HideMainWindow();
        void ShowMainWindow();
        void ShutDown();
    }
}