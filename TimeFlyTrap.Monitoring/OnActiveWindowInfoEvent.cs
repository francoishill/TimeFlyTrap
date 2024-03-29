using System;

// ReSharper disable MemberCanBePrivate.Global

namespace TimeFlyTrap.Monitoring
{
    public class OnActiveWindowInfoEvent
    {
        public DateTime Time { get; }
        public string Title { get; }
        public string ModuleFilePath { get; }
        public DateTime SystemStartupTime { get; }
        public TimeSpan UserIdleDuration { get; }

        public OnActiveWindowInfoEvent(DateTime time, string title, string moduleFilePath, DateTime systemStartupTime, TimeSpan userIdleDuration)
        {
            Time = time;
            Title = title;
            ModuleFilePath = moduleFilePath;
            SystemStartupTime = systemStartupTime;
            UserIdleDuration = userIdleDuration;
        }
    }
}