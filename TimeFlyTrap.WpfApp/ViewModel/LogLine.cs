using System;
using Microsoft.Extensions.Logging;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class LogLine
    {
        public string TimeString { get; }
        public string Text { get; }
        public LogLevel Level { get; }

        public LogLine(string text, LogLevel level, DateTime? time = null)
        {
            TimeString = (time ?? DateTime.Now).ToString("yyyy-MM-dd HH:mm:ss");
            Text = text;
            Level = level;
        }

        public bool IsTrace => Level == LogLevel.Trace;
        public bool IsDebug => Level == LogLevel.Debug;
        public bool IsWarning => Level == LogLevel.Warning;
        public bool IsError => Level == LogLevel.Error;
        public bool IsCritical => Level == LogLevel.Critical;

        public string DisplayText => $"[{Level.ToString()[0]} | {TimeString}] {Text}";
    }
}