using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace PlayAccumulateTimeFlyTrap.Models
{
    //TODO: Duplicate in TimeFlyTrap.Monitoring project
    [DebuggerDisplay("IdleDuration = {IdleDuration}, TotalDuration = {TotalDuration}")]
    public class WindowTimes
    {
        public string WindowTitle { get; set; }
        public string ProcessPath { get; set; }
        public Dictionary<DateTime, DateTime> IdleTimes { get; set; }
        public Dictionary<DateTime, DateTime> TotalTimes { get; set; }

        public TimeSpan IdleDuration => TimeSpan.FromSeconds(IdleTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0));
        public TimeSpan TotalDuration => TimeSpan.FromSeconds(TotalTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0));
        public TimeSpan ActiveDuration => TotalDuration - IdleDuration;

        public int IdleTimesCount => IdleTimes != null ? IdleTimes.Count : 0;
        public int TotalTimesCount => TotalTimes != null ? TotalTimes.Count : 0;
    }
}