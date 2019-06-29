using System.Collections.Generic;

namespace TimeFlyTrap.WpfApp.Settings
{
    public class AppSettings
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, string> PatternsWithTitleAlias { get; set; } = new Dictionary<string, string>();
    }
}