using System.Collections.Generic;
using PlayAccumulateTimeFlyTrap.Models;

namespace PlayAccumulateTimeFlyTrap.Services
{
    public interface IMainService
    {
        IEnumerable<WindowTimes> LoadWindowTimes();
    }
}