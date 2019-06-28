using System.Windows;
using TimeFlyTrap.WpfApp.Domain.Services;

namespace TimeFlyTrap.WpfApp.Services
{
    public class MainWindowProvider : IMainWindowProvider
    {
        public Window Window => Application.Current.MainWindow;
    }
}