using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using Newtonsoft.Json;

namespace TimeFlyTrap.Monitoring
{
    public class ActiveWindowsTracker : IActiveWindowsTracker
    {
        private const string NULL_WINDOW_TITLE = "[NULLWINDOWTITLE]";
        private const string NULL_FILE_PATH = "[NULLFILEPATH]";
        private static readonly TimeSpan _minimumIdleDuration = TimeSpan.FromSeconds(5);

        private readonly Dictionary<string, WindowTimes> _activatedWindowsAndTimes;
        private readonly Timer _ticker;
        private bool _tickerRunning;

        private string _activeWindowTitle;
        private string _activeWindowModuleFilepath;
        private string _activeWindowString;

        private string _lastWindowString;
        private string _lastIdleWindowString;
        private bool _isBusy;

        public ActiveWindowsTracker()
        {
            _activatedWindowsAndTimes = new Dictionary<string, WindowTimes>(StringComparer.InvariantCultureIgnoreCase);

            _ticker = new Timer
            {
                Interval = Constants.TickerInterval.TotalMilliseconds,
                AutoReset = true
            };
        }

        public void StartTicker(ITrackingListener trackingListener)
        {
            if (_tickerRunning)
            {
                throw new InvalidOperationException("Cannot call StartTicker more than once");
            }

            _ticker.Elapsed += delegate { OnTicker(trackingListener); };

            _ticker.Start();
            _tickerRunning = true;
        }

        private void OnTicker(ITrackingListener trackingListener)
        {
            DateTime systemStartupTime;
            TimeSpan userIdleDuration;

            if (!Win32.GetLastInputInfo(out systemStartupTime, out userIdleDuration))
            {
                return;
            }

            if (_isBusy)
            {
                return;
            }

            _isBusy = true;
            try
            {
                RecordMeasurement(trackingListener, systemStartupTime, userIdleDuration);
            }
            finally
            {
                _isBusy = false;
            }
        }

        private void RecordMeasurement(ITrackingListener trackingListener, DateTime systemStartupTime, TimeSpan userIdleDuration)
        {
            var now = DateTime.Now;
            _activeWindowTitle = GetActiveWindowTitle() ?? NULL_WINDOW_TITLE;
            _activeWindowModuleFilepath = GetActiveWindowModuleFilePath() ?? NULL_FILE_PATH;
            _activeWindowString = _activeWindowTitle + "|" + _activeWindowModuleFilepath;

            trackingListener.OnActiveWindowInfo(new OnActiveWindowInfoEvent(now, _activeWindowTitle, _activeWindowModuleFilepath, systemStartupTime, userIdleDuration));

            if (!_activatedWindowsAndTimes.ContainsKey(_activeWindowString))
            {
                _activatedWindowsAndTimes.Add(_activeWindowString, new WindowTimes(_activeWindowTitle, _activeWindowModuleFilepath));
            }

            var winTimes = _activatedWindowsAndTimes[_activeWindowString];

            if (!_activeWindowString.Equals(_lastWindowString, StringComparison.InvariantCultureIgnoreCase))
            {
                _lastWindowString = _activeWindowString;

                winTimes.TotalTimes.Add(now, DateTime.MinValue);
            }

            var lastStartTime = winTimes.TotalTimes.Last().Key;
            winTimes.TotalTimes[lastStartTime] = now;

            if (userIdleDuration.TotalSeconds > _minimumIdleDuration.TotalSeconds)
            {
                if (!_activeWindowString.Equals(_lastIdleWindowString, StringComparison.InvariantCultureIgnoreCase))
                {
                    _lastIdleWindowString = _activeWindowString;
                    winTimes.IdleTimes.Add(now.Subtract(userIdleDuration), DateTime.MinValue);
                }

                var lastIdleStart = winTimes.IdleTimes.Last().Key;
                winTimes.IdleTimes[lastIdleStart] = now;
            }
            else
            {
                _lastIdleWindowString = null;
            }
        }

        public void Stop()
        {
            if (!_tickerRunning)
                return;

            _ticker.Stop();
            _tickerRunning = false;
            var now = DateTime.Now;

            if (_lastWindowString != null && _activatedWindowsAndTimes.ContainsKey(_lastWindowString))
            {
                var wintimes = _activatedWindowsAndTimes[_lastWindowString];
                if (wintimes.TotalTimes.Count > 0)
                {
                    var lastTime = wintimes.TotalTimes.Last().Key;
                    wintimes.TotalTimes[lastTime] = now;
                }
            }

            _lastWindowString = null;

            if (_lastIdleWindowString != null && _activatedWindowsAndTimes.ContainsKey(_lastIdleWindowString))
            {
                var wintimesidle = _activatedWindowsAndTimes[_lastIdleWindowString];
                if (wintimesidle.IdleTimes.Count > 0)
                {
                    var lastIdleTime = wintimesidle.IdleTimes.Last().Key;
                    wintimesidle.IdleTimes[lastIdleTime] = now;
                }
            }

            _lastIdleWindowString = null;
        }

//        public bool StopAndGetReport(out Dictionary<string, WindowTimes> activatedWindowsList)
//        {
//            Stop();
//            return GetReport(out activatedWindowsList);
//        }

        public Dictionary<string, WindowTimes> GetReport(bool clearList)
        {
            lock (_activatedWindowsAndTimes)
            {
                var list = _activatedWindowsAndTimes
                    .ToList() // clone
                    .OrderBy(kv => -kv.Value.TotalTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0))
                    .ToDictionary(kv => kv.Key, kv => kv.Value);

                if (clearList)
                {
                    _activatedWindowsAndTimes.Clear();
                }

                return list;
            }
        }

        public static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            IntPtr handle;
            var buff = new StringBuilder(nChars);
            handle = Win32.GetForegroundWindow();

            if (Win32.GetWindowText(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }

            return null;
        }

        public static string GetActiveWindowModuleFilePath()
        {
            IntPtr handle;
            handle = Win32.GetForegroundWindow();

            var file = Win32.GetWindowProcessFilePath(handle);
            if (!string.IsNullOrWhiteSpace(file))
            {
                return file;
            }

            const int nChars = 256;
            var buff = new StringBuilder(nChars);
            if (Win32.GetWindowModuleFileName(handle, buff, nChars) > 0)
            {
                return buff.ToString();
            }

            return null;
        }

        public static ObservableCollection<WindowTimes> LoadReportsFromJson(string filepath, int filterByMinimumSeconds, List<string> groupingWindowTitlesBySubstring)
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    throw new Exception("File does not exist: " + filepath);
                }

                var jsonText = File.ReadAllText(filepath);
                var tmpWrappedList = JsonConvert.DeserializeObject<WindowTimes[]>(jsonText);

                var tmpList = new ObservableCollection<WindowTimes>(tmpWrappedList);
                PopulateList(ref tmpList, filterByMinimumSeconds, groupingWindowTitlesBySubstring);

                return new ObservableCollection<WindowTimes>(tmpList
                    .OrderBy(it => -it.TotalTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0)));
            }
            catch (Exception exc)
            {
                throw new Exception(
                    "Error loading report file: " + Environment.NewLine + exc.Message
                    + Environment.NewLine + Environment.NewLine + "Stacktrace:" + Environment.NewLine + exc.StackTrace);
            }
        }

        public static void PopulateList(ref ObservableCollection<WindowTimes> wintimes, int minimumSeconds, List<string> groupingWindowTitlesBySubstring)
        {
            if (wintimes == null) return;

            if (groupingWindowTitlesBySubstring != null)
                foreach (var grp in groupingWindowTitlesBySubstring)
                {
                    WindowTimes wintime = null;
                    for (var i = wintimes.Count - 1; i >= 0; i--)
                    {
                        if (wintimes[i].WindowTitle.IndexOf(grp, StringComparison.InvariantCultureIgnoreCase) != -1)
                        {
                            if (wintime == null)
                                wintime = new WindowTimes("... " + grp + " ...", NULL_FILE_PATH);
                            foreach (var tottime in wintimes[i].TotalTimes)
                                if (!wintime.TotalTimes.ContainsKey(tottime.Key))
                                    wintime.TotalTimes.Add(tottime.Key, tottime.Value);
                            foreach (var idletime in wintimes[i].IdleTimes)
                                if (!wintime.IdleTimes.ContainsKey(idletime.Key))
                                    wintime.IdleTimes.Add(idletime.Key, idletime.Value);
                            wintimes.RemoveAt(i);
                        }
                    }

                    if (wintime != null)
                        wintimes.Add(wintime);
                }

            wintimes = new ObservableCollection<WindowTimes>(
                wintimes
                    .OrderBy(it => -it.TotalTimes.Sum(dateDur => dateDur.Value != DateTime.MinValue ? (dateDur.Value.Subtract(dateDur.Key).TotalSeconds) : 0))
                    .Where(wt => wt.TotalSeconds >= minimumSeconds));
        }

        public static void SaveReportsToJsonAndHtml(IEnumerable<WindowTimes> reportList, string jsonFilepath, string htmlFilepath /*, string recordingSaveToDirectory*/)
        {
            var reportListArray = reportList as WindowTimes[] ?? reportList.ToArray();

            if (reportList == null || reportListArray.Length == 0)
            {
                throw new Exception("There are no reports to save");
            }

            if (File.Exists(jsonFilepath))
            {
                File.Delete(jsonFilepath);
            }

            File.WriteAllText(jsonFilepath, JsonConvert.SerializeObject(reportListArray.ToList()));

            var htmltext = "";
            htmltext += "<html>";
            htmltext += "<head>";

            htmltext += "<style>";
            htmltext += "td { vertical-align: top; }";
            htmltext += ".title{ color: blue; }";
            htmltext += ".totaltime{ color: green; }";
            htmltext += ".idletime{ color: orange; }";
            htmltext += ".fullpath{ color: gray; font-size: 10px; }";
            htmltext += "</style>";

            htmltext += "</head>";
            htmltext += "<body>";

            htmltext += "<table cellspacing='0' border='1'>";
            htmltext += "<thead><th>Window Title</th><th>Total seconds</th><th>Idle seconds</th><th>Fullpath</th></thead>";

            foreach (var rep in reportListArray)
                htmltext +=
                    "<tr>" +
                    string.Format(
                        "<td class='title'>{0}</td><td class='totaltime'>{1}</td><td class='idletime'>{2}</td><td class='fullpath'>{3}</td>",
                        rep.WindowTitle,
                        string.Join("<br/>", rep.IdleTimes.Select(idl => idl.Key.ToString("yyyy-MM-dd HH:mm:ss") + " for " + (idl.Value != DateTime.MinValue ? (idl.Value.Subtract(idl.Key).TotalSeconds) : 0) + " seconds")),
                        string.Join("<br/>", rep.TotalTimes.Select(idl => idl.Key.ToString("yyyy-MM-dd HH:mm:ss") + " for " + (idl.Value != DateTime.MinValue ? (idl.Value.Subtract(idl.Key).TotalSeconds) : 0) + " seconds")),
                        rep.ProcessPath)
                    + "</tr>";

            htmltext += "</table>";
            htmltext += "</body></html>";
            File.WriteAllText(htmlFilepath, htmltext);

            Process.Start("explorer", "/select,\"" + htmlFilepath + "\"");
        }
    }
}