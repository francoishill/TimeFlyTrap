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
        private readonly IAppFileSystem _appFileSystem;
        private readonly IActiveWindowsTracker _activeWindowsTracker;
        private ICollection<WindowTimes> _reportTimes;

        public ReportViewModel(
            IMessenger messenger,
            IFileChooserFactory fileChooserFactory,
            IAppManager appManager,
            IAppFileSystem appFileSystem,
            IActiveWindowsTracker activeWindowsTracker)
        {
            _fileChooserFactory = fileChooserFactory;
            _appManager = appManager;
            _appFileSystem = appFileSystem;
            _activeWindowsTracker = activeWindowsTracker;

            messenger.Register<ChooseJsonFileDialogEvent>(this, OnChooseJsonFileDialog);
            messenger.Register<ShowCurrentReportEvent>(this, OnShowCurrentReport);
            messenger.Register<SaveCurrentReportEvent>(this, OnSaveCurrentReport);
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

        private void OnShowCurrentReport(ShowCurrentReportEvent @event)
        {
            if (_activeWindowsTracker.GetReport(out var activeWindowsList))
            {
                ReportTimes = activeWindowsList.Values;
            }
        }

        private void OnSaveCurrentReport(SaveCurrentReportEvent @event)
        {
            if (_activeWindowsTracker.GetReport(out var activeWindowsList))
            {
                _appFileSystem.WriteReport(activeWindowsList);
            }
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