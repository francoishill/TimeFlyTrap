using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain.ViewModels;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class ActiveWindowTrackerViewModel : ViewModelBase, IActiveWindowTrackerViewModel
    {
        private readonly IActiveWindowsTracker _activeWindowsTracker;
        private readonly object _logLinesLock = new object();
        private readonly Queue<string> _logLines = new Queue<string>();

        public ActiveWindowTrackerViewModel(IActiveWindowsTracker activeWindowsTracker)
        {
            _activeWindowsTracker = activeWindowsTracker;
        }

        // ReSharper disable once InconsistentlySynchronizedField
        public string LogText => string.Join(Environment.NewLine, _logLines);

        public void StartTracking()
        {
            _activeWindowsTracker.StartTicker(new TrackingListener(this));
        }

        private void AppendLine(string text)
        {
            lock (_logLinesLock)
            {
                _logLines.Enqueue(text);

                while (_logLines.Count > Constants.MAX_LOG_LINES)
                {
                    _logLines.Dequeue();
                }

                RaisePropertyChanged(nameof(LogText));
            }
        }

        private class TrackingListener : ITrackingListener
        {
            private readonly ActiveWindowTrackerViewModel _viewModel;

            public TrackingListener(ActiveWindowTrackerViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public void OnLastInfo(OnLastInfoEvent @event)
            {
                _viewModel.AppendLine($"Startup: {@event.SystemStartupTime}, IdleDuration: {@event.IdleDuration}");
            }

            public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
            {
                _viewModel.AppendLine($"New title: {@event.Title}, Module: {@event.ModuleFilePath}");
            }
        }
    }
}