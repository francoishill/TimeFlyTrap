using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Events;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class NotifyIconViewModel : ViewModelBase, INotifyIconViewModel
    {
        private readonly IAppManager _appManager;
        private readonly IMessenger _messenger;
        private bool _showNotifyIcon = true;

        private bool _showWindow = Constants.SHOW_WINDOW_ON_STARTUP;

        public NotifyIconViewModel(IAppManager appManager, IMessenger messenger)
        {
            _appManager = appManager;
            _messenger = messenger;

            ChooseJsonFileDialogCommand = new RelayCommand(OnChooseJsonFileDialog);
            ShowCurrentReportCommand = new RelayCommand(OnShowCurrentReport);
            SaveCurrentReportCommand = new RelayCommand(OnSaveCurrentReport);
            ExitCommand = new RelayCommand(OnExit);

            HideWindowCommand = new RelayCommand(OnHideWindow);
            ShowWindowCommand = new RelayCommand(OnShowWindow);

            _messenger.Register<MainWindowLoadedEvent>(this, OnMainWindowLoaded);
        }

        public ICommand ChooseJsonFileDialogCommand { get; }
        public ICommand ShowCurrentReportCommand { get; }
        public ICommand SaveCurrentReportCommand { get; }
        public ICommand ExitCommand { get; }

        public ICommand HideWindowCommand { get; }
        public ICommand ShowWindowCommand { get; }

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

        public bool ShowWindow
        {
            get => _showWindow;
            set
            {
                if (_showWindow == value) return;
                _showWindow = value;
                RaisePropertyChanged(nameof(ShowWindow));
            }
        }

        private void OnMainWindowLoaded(MainWindowLoadedEvent @event)
        {
            if (!ShowWindow)
            {
                OnHideWindow();
            }
        }

        private void OnHideWindow()
        {
            ShowWindow = false;
            _appManager.HideMainWindow();
        }

        private void OnShowWindow()
        {
            ShowWindow = true;
            _appManager.ShowMainWindow();
        }

        private void OnChooseJsonFileDialog()
        {
            _messenger.Send(new ChooseJsonFileDialogEvent());
        }

        private void OnShowCurrentReport()
        {
            _messenger.Send(new ShowCurrentReportEvent());
        }

        private void OnSaveCurrentReport()
        {
            _messenger.Send(new SaveCurrentReportEvent());
        }

        private void OnExit()
        {
            ShowNotifyIcon = false;
            _appManager.ShutDown();
        }
    }
}