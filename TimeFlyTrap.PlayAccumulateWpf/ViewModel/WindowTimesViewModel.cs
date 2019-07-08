using System;
using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.ViewModel
{
    public class WindowTimesViewModel
    {
        public TimeSpan TotalDuration { get; }
        public TimeSpan IdleDuration { get; }
        public TimeSpan ActiveDuration { get; }
        public string FormattedWindowTitle { get; }
        public WindowTimes[] SeparateModels { get; set; }

        public WindowTimesViewModel(string formattedWindowTitle, TimeSpan totalDuration, TimeSpan idleDuration, TimeSpan activeDuration, WindowTimes[] separateModels)
        {
            FormattedWindowTitle = formattedWindowTitle;
            TotalDuration = totalDuration;
            IdleDuration = idleDuration;
            ActiveDuration = activeDuration;
            SeparateModels = separateModels;
        }

        public string DisplayText => $"[active: {FormatTimeSpan(ActiveDuration)}, idle: {FormatTimeSpan(IdleDuration)}] {FormattedWindowTitle}";

        private static string FormatTimeSpan(TimeSpan timeSpan) => $"{timeSpan:d\\d\\ hh\\h\\ mm\\m\\ ss}";
    }
}