using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Events;

// ReSharper disable MemberCanBePrivate.Global

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class ReportViewModel : ViewModelBase, IReportViewModel
    {
        private readonly IFileChooserFactory _fileChooserFactory;
        private readonly IAppManager _appManager;
        private ICollection<WindowTimes> _reportTimes;

        public ReportViewModel(IMessenger messenger, IFileChooserFactory fileChooserFactory, IAppManager appManager)
        {
            _fileChooserFactory = fileChooserFactory;
            _appManager = appManager;

            messenger.Register<ChooseJsonFileDialogEvent>(this, OnChooseJsonFileDialog);
        }

        public void OnChooseJsonFileDialog(ChooseJsonFileDialogEvent @event)
        {
            var fileChooser = _fileChooserFactory.Create();
            if (!fileChooser.Choose(FileType.ReportJson) || !fileChooser.DoesChosenFileExist)
            {
                return;
            }

            const int tmpMinSecs = 1;
            var groupingWindowTitlesBySubstring = new List<string>();
            var winTimes = ActiveWindowsTracker.LoadReportsFromJson(fileChooser.ChosenFile, tmpMinSecs, groupingWindowTitlesBySubstring);
            ReportTimes = winTimes;

            _appManager.ShowMainWindow();
        }

        public ICollection<WindowTimes> ReportTimes
        {
            get => _reportTimes;
            set
            {
                // ReSharper disable once PossibleUnintendedReferenceComparison
                if (_reportTimes == value) return;
                _reportTimes = value;
                RaisePropertyChanged(nameof(ReportTimes));
            }
        }
    }
}