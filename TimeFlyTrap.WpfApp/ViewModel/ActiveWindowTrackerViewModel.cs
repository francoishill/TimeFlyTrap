using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.Logging;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain;
using TimeFlyTrap.WpfApp.Domain.ViewModels;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class ActiveWindowTrackerViewModel : ViewModelBase, IActiveWindowTrackerViewModel
    {
        private readonly IActiveWindowsTracker _activeWindowsTracker;
        private readonly ISettingsProvider _settingsProvider;
        private readonly object _logLinesLock = new object();
        private readonly Queue<string> _logLines = new Queue<string>();

        public ActiveWindowTrackerViewModel(
            IActiveWindowsTracker activeWindowsTracker,
            ISettingsProvider settingsProvider)
        {
            _activeWindowsTracker = activeWindowsTracker;
            _settingsProvider = settingsProvider;
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
                _viewModel.OnLastInfo(@event);
            }

            public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
            {
                _viewModel.OnActiveWindowInfo(@event);
            }
        }

        private void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
        {
            var formattedTitle = FormatTitle(@event.Title);
            AppendLine($"New title: {formattedTitle}, Module: {@event.ModuleFilePath}");
        }

        private void OnLastInfo(OnLastInfoEvent @event)
        {
            AppendLine($"Startup: {@event.SystemStartupTime}, IdleDuration: {@event.IdleDuration}");
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            AppendLine($"[{logLevel.ToString()}] {eventId} {formatter(state, exception)}");
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