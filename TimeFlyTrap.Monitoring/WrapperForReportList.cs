using System.Collections.Generic;

namespace TimeFlyTrap.Monitoring
{
    public class WrapperForReportList
    {
        public readonly List<WindowTimes> ListOfReports;

        public WrapperForReportList()
        {
        }

        public WrapperForReportList(List<WindowTimes> listOfReports)
        {
            ListOfReports = listOfReports;
        }
    }
}