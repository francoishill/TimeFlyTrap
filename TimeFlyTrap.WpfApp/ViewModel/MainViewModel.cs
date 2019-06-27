using System;
using System.IO;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;

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
            ChooseJsonFileDialogCommand = new RelayCommand(OnOpenDialog);
        }

        private void OnOpenDialog()
        {
            var initialDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimeFlyTrap");
            if (!Directory.Exists(initialDir))
            {
                Directory.CreateDirectory(initialDir);
            }

            var openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = initialDir;
            if (openFileDialog.ShowDialog() == true && File.Exists(openFileDialog.FileName))
            {
                Report = new ReportViewModel(openFileDialog.FileName);
            }
        }

        public ICommand ChooseJsonFileDialogCommand { get; }

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