using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
        }

        public static string GenerateHtml(Dictionary<string, WindowTimes> reportList)
        {
            var htmlText = new StringBuilder();
            htmlText.Append("<html>");
            htmlText.Append("<head>");

            htmlText.Append("<style>");
            htmlText.Append("td { vertical-align: top; }");
            htmlText.Append(".title{ color: blue; }");
            htmlText.Append(".totaltime{ color: green; }");
            htmlText.Append(".idletime{ color: orange; }");
            htmlText.Append(".fullpath{ color: gray; font-size: 10px; }");
            htmlText.Append("</style>");

            htmlText.Append("</head>");
            htmlText.Append("<body>");

            htmlText.Append("<table cellspacing='0' border='1'>");
            htmlText.Append("<thead><th>Window Title</th><th>Total seconds</th><th>Idle seconds</th><th>Fullpath</th></thead>");

            foreach (var rep in reportList.Values)
            {
                htmlText.Append(
                    "<tr>" +
                    string.Format(
                        "<td class='title'>{0}</td><td class='totaltime'>{1}</td><td class='idletime'>{2}</td><td class='fullpath'>{3}</td>",
                        rep.WindowTitle,
                        string.Join("<br/>", rep.IdleTimes.Select(idl => idl.Key.ToString("yyyy-MM-dd HH:mm:ss") + " for " + (idl.Value != DateTime.MinValue ? (idl.Value.Subtract(idl.Key).TotalSeconds) : 0) + " seconds")),
                        string.Join("<br/>", rep.TotalTimes.Select(idl => idl.Key.ToString("yyyy-MM-dd HH:mm:ss") + " for " + (idl.Value != DateTime.MinValue ? (idl.Value.Subtract(idl.Key).TotalSeconds) : 0) + " seconds")),
                        rep.ProcessPath)
                    + "</tr>");
            }

            htmlText.Append("</table>");
            htmlText.Append("</body></html>");

            return htmlText.ToString();
        }
    }
}