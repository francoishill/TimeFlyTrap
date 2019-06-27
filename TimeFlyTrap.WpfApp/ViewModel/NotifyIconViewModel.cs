using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using TimeFlyTrap.WpfApp.Events;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class NotifyIconViewModel : ViewModelBase
    {
        private bool _showNotifyIcon = true;

        public bool ShowNotifyIcon
        {
            get => _showNotifyIcon;
            set
            {
                if (_showNotifyIcon == value) return;
                _showNotifyIcon = value;
                RaisePropertyChanged(nameof(ShowNotifyIcon));
            }
        }

        public ICommand ChooseJsonFileDialogCommand { get; }
        public ICommand ExitCommand { get; }

        public NotifyIconViewModel()
        {
            ChooseJsonFileDialogCommand = new RelayCommand(OnChooseJsonFileDialog);
            ExitCommand = new RelayCommand(OnExit);
        }

        private void OnChooseJsonFileDialog()
        {
            MessengerInstance.Send(new ChooseJsonFileDialogEvent());
        }

        private void OnExit()
        {
            MessengerInstance.Send(new ExitApplicationEvent());
        }
    }
}