using System.Collections.Generic;
using TimeFlyTrap.Monitoring;

namespace TimeFlyTrap.WpfApp.Domain.ViewModels
{
    public interface IReportViewModel
    {
        ICollection<WindowTimes> ReportTimes { get; set; }
    }
}