using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TimeFlyTrap.Monitoring
{
    [DebuggerDisplay("IdleSeconds = {IdleSeconds}, TotalSeconds = {TotalSeconds}")]
    public class WindowTimes
    {
        public string WindowTitle { get; set; }
        public string ProcessPath { get; set; }
        public Dictionary<DateTime, DateTime> IdleTimes { get; set; }
        public Dictionary<DateTime, DateTime> TotalTimes { get; set; }

        public long IdleSeconds
        {
            get { return (long) IdleTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0); }
        }

        public long TotalSeconds
        {
            get { return (long) TotalTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0); }
        }

        public int IdleTimesCount
        {
            get { return IdleTimes != null ? IdleTimes.Count : 0; }
        }

        public int TotalTimesCount
        {
            get { return TotalTimes != null ? TotalTimes.Count : 0; }
        }
        //public long IdleSeconds { get; set; }
        //public long TotalSeconds { get; set; }

        public List<string> IdleTimeStrings
        {
            get
            {
                return
                    IdleTimes
                        .Where(kv => kv.Value != DateTime.MinValue)
                        .Select(kv => kv.Key.ToString("HH:mm:ss") + " - " + kv.Value.ToString("HH:mm:ss") + " (total seconds = " + kv.Value.Subtract(kv.Key).TotalSeconds + ")")
                        .ToList();
            }
        }

        public List<string> TotalTimeStrings
        {
            get
            {
                return
                    TotalTimes
                        .Where(kv => kv.Value != DateTime.MinValue)
                        .Select(kv => kv.Key.ToString("HH:mm:ss") + " - " + kv.Value.ToString("HH:mm:ss") + " (total seconds = " + kv.Value.Subtract(kv.Key).TotalSeconds + ")")
                        .ToList();
            }
        }

        public WindowTimes()
        {
        } //Must be here for json parser to create it

        public WindowTimes(string windowTitle, string processPath)
        {
            WindowTitle = windowTitle;
            ProcessPath = processPath;
            IdleTimes = new Dictionary<DateTime, DateTime>();
            TotalTimes = new Dictionary<DateTime, DateTime>();
            //this.IdleSeconds = 0;
            //this.TotalSeconds = 0;
        }
    }
}