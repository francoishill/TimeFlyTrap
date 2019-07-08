using System.Collections.Generic;
using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.Services
{
    public class DesignMainService : IMainService
    {
        public IEnumerable<WindowTimes> LoadWindowTimes()
        {
            return new WindowTimes[0];
        }
    }
}