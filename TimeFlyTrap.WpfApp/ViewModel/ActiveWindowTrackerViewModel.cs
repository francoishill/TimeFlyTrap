using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Logging;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.ViewModels;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class ActiveWindowTrackerViewModel : ViewModelBase, IActiveWindowTrackerViewModel
    {
        private readonly ILogger<ActiveWindowTrackerViewModel> _logger;
        private readonly IActiveWindowsTracker _activeWindowsTracker;
        private readonly ISettingsProvider _settingsProvider;
        private readonly IApiUploader _apiUploader;

        private readonly object _logLinesLock = new object();

        public ActiveWindowTrackerViewModel(
            ILogger<ActiveWindowTrackerViewModel> logger,
            IActiveWindowsTracker activeWindowsTracker,
            ISettingsProvider settingsProvider,
            IApiUploader apiUploader)
        {
            _logger = logger;
            _activeWindowsTracker = activeWindowsTracker;
            _settingsProvider = settingsProvider;
            _apiUploader = apiUploader;
        }

        public ObservableCollection<LogLine> LogLines { get; } = new ObservableCollection<LogLine>();

        public void StartTracking()
        {
            _activeWindowsTracker.StartTicker(new TrackingListener(this));
        }

        private void AppendLine(LogLine line)
        {
            _logger.Log(line.Level, 0, line.Text);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                lock (_logLinesLock)
                {
                    LogLines.Insert(0, line);

                    while (LogLines.Count > Constants.MAX_LOG_LINES)
                    {
                        LogLines.RemoveAt(LogLines.Count - 1);
                    }

                    RaisePropertyChanged(nameof(LogLines));
                }
            });
        }

        private class TrackingListener : ITrackingListener
        {
            private readonly ActiveWindowTrackerViewModel _viewModel;

            public TrackingListener(ActiveWindowTrackerViewModel viewModel)
            {
                _viewModel = viewModel;
            }

            public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
            {
                _viewModel.OnActiveWindowInfo(@event);
            }
        }

        private void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
        {
            var formattedTitle = FormatTitle(@event.Title);
            AppendLine(new LogLine($"New title: {formattedTitle}, Module: {@event.ModuleFilePath}, Startup: {@event.SystemStartupTime}, IdleDuration: {@event.UserIdleDuration}", LogLevel.Debug));
            _apiUploader.OnActiveWindowInfo(@event);
        }

        private void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            AppendLine(new LogLine($"[{logLevel.ToString()}] {eventId} {formatter(state, exception)}", logLevel));
        }

        private string FormatTitle(string title)
        {
            var matches = _settingsProvider.Settings.PatternsWithTitleAlias
                .Select(patternAndAlias => (Pattern: patternAndAlias.Key, Alias: patternAndAlias.Value, Match: new Regex(patternAndAlias.Key).Match(title)))
                .Where(x => x.Match.Success)
                .ToArray();

            if (matches.Length == 0)
            {
                return title;
            }

            if (matches.Length > 1)
            {
                Log(
                    LogLevel.Error,
                    0,
                    $"Multiple patterns match title '{title}', using first:\n{string.Join(Environment.NewLine, matches.Select(x => x.Pattern))}",
                    null,
                    (s, ex) => s);
            }

            var (_, @alias, firstMatch) = matches[0];

            var newTitle = @alias;

            for (var i = 0; i < firstMatch.Groups.Count; i++)
            {
                if (i == 0)
                {
                    continue; // The full match
                }
                newTitle = newTitle.Replace("{" + (i - 1) + "}", firstMatch.Groups[i].Value.Trim());
            }
            return newTitle;
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
        {
            return new NopDisposable();
        }

        private class NopDisposable : IDisposable
        {
            public void Dispose()
            {
                // Nothing
            }
        }
    }
}