using System.Windows;
using TimeFlyTrap.WpfApp.Domain.Services;

namespace TimeFlyTrap.WpfApp.Services
{
    public class AppManager : IAppManager
    {
        private readonly IMainWindowProvider _mainWindowProvider;
        private readonly IApiUploader _apiUploader;

        public AppManager(IMainWindowProvider mainWindowProvider, IApiUploader apiUploader)
        {
            _mainWindowProvider = mainWindowProvider;
            _apiUploader = apiUploader;
        }

        public void HideMainWindow()
        {
            _mainWindowProvider.Window.Hide();
            _mainWindowProvider.Window.WindowState = WindowState.Minimized;
        }

        public void ShowMainWindow()
        {
            _mainWindowProvider.Window.Show();
            _mainWindowProvider.Window.WindowState = WindowState.Maximized;
        }

        public void ShutDown()
        {
            _apiUploader.StopUploading();
            Application.Current.Shutdown();
        }
    }
}