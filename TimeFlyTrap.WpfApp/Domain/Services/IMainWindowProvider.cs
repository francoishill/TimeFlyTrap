using System.Windows;

namespace TimeFlyTrap.WpfApp.Domain.Services
{
    public interface IMainWindowProvider
    {
        Window Window { get; }
    }
}