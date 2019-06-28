using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Events;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class NotifyIconViewModel : ViewModelBase, INotifyIconViewModel
    {
        private readonly IMessenger _messenger;
        private bool _showNotifyIcon = true;

        public NotifyIconViewModel(IMessenger messenger)
        {
            _messenger = messenger;

            ChooseJsonFileDialogCommand = new RelayCommand(OnChooseJsonFileDialog);
            ExitCommand = new RelayCommand(OnExit);
        }

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

        private void OnChooseJsonFileDialog()
        {
            _messenger.Send(new ChooseJsonFileDialogEvent());
        }

        private void OnExit()
        {
            _messenger.Send(new ExitApplicationEvent());
        }
    }
}