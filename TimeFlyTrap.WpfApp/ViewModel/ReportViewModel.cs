using System.Collections.Generic;
using GalaSoft.MvvmLight;
using TimeFlyTrap.Monitoring;

// ReSharper disable MemberCanBePrivate.Global

namespace TimeFlyTrap.WpfApp.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        public ReportViewModel(string filePath)
        {
            const int tmpMinSecs = 1;
            var groupingWindowTitlesBySubstring = new List<string>();
            var winTimes = ActiveWindowsTracker.LoadReportsFromJson(filePath, tmpMinSecs, groupingWindowTitlesBySubstring);
            ReportTimes = winTimes;
        }

        public ICollection<WindowTimes> ReportTimes { get; set; }
    }
}