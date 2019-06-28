using System;

namespace TimeFlyTrap.Monitoring
{
    public class OnLastInfoEvent
    {
        public DateTime SystemStartupTime { get; }
        public TimeSpan IdleDuration { get; }

        public OnLastInfoEvent(DateTime systemStartupTime, TimeSpan idleDuration)
        {
            SystemStartupTime = systemStartupTime;
            IdleDuration = idleDuration;
        }
    }
}