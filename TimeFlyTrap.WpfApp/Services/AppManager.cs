using System.Windows;
using TimeFlyTrap.WpfApp.Domain.Services;

namespace TimeFlyTrap.WpfApp.Services
{
    public class AppManager : IAppManager
    {
        public void ShutDown()
        {
            Application.Current.Shutdown();
        }
    }
}