using System.Windows.Input;

namespace TimeFlyTrap.WpfApp.Domain.ViewModels
{
    public interface INotifyIconViewModel
    {
        bool ShowNotifyIcon { get; set; }
        ICommand ChooseJsonFileDialogCommand { get; }
        ICommand ExitCommand { get; }
    }
}