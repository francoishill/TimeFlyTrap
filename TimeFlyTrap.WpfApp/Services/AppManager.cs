using System.Windows;
using TimeFlyTrap.WpfApp.Domain.Services;

namespace TimeFlyTrap.WpfApp.Services
{
    public class AppManager : IAppManager
    {
        private readonly IMainWindowProvider _mainWindowProvider;

        public AppManager(IMainWindowProvider mainWindowProvider)
        {
            _mainWindowProvider = mainWindowProvider;
        }

        public void HideMainWindow()
        {
            _mainWindowProvider.Window.Hide();
        }

        public void ShowMainWindow()
        {
            _mainWindowProvider.Window.Show();
        }

        public void ShutDown()
        {
            Application.Current.Shutdown();
        }
    }
}