using System.Collections.Generic;
using TimeFlyTrap.Monitoring;

namespace TimeFlyTrap.WpfApp.Domain.Services.FileSystem
{
    public interface IAppFileSystem
    {
        string LocalAppData { get; }

        bool ReadSettingsFile(out string settingsContent);
        void SaveSettings(string settingsContent);

        void WriteReport(Dictionary<string, WindowTimes> values);
    }
}