using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Win32;
using TimeFlyTrap.WpfApp.Events;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        private ReportViewModel m_Report;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            NotifyIcon = new NotifyIconViewModel();
            
            MessengerInstance.Register<ExitApplicationEvent>(this, OnExitApplication);
            MessengerInstance.Register<ChooseJsonFileDialogEvent>(this, OnChooseJsonFileDialog);
        }

        private void OnExitApplication(ExitApplicationEvent @event)
        {
            NotifyIcon.ShowNotifyIcon = false;
            Application.Current.Shutdown();
        }

        private void OnChooseJsonFileDialog(ChooseJsonFileDialogEvent @event)
        {
            var initialDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeFlyTrap");
            if (!Directory.Exists(initialDir))
            {
                Directory.CreateDirectory(initialDir);
            }

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = initialDir;

            if (openFileDialog.ShowDialog(Application.Current.MainWindow) == true 
                && File.Exists(openFileDialog.FileName))
            {
                Report = new ReportViewModel(openFileDialog.FileName);
            }
        }

        public NotifyIconViewModel NotifyIcon { get; }

        public ReportViewModel Report
        {
            get => m_Report;
            set
            {
                if (m_Report == value) return;

                m_Report = value;
                RaisePropertyChanged(nameof(Report));
            }
        }
    }
}