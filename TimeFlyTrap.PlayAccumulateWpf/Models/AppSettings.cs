using System.Collections.Generic;

namespace PlayAccumulateTimeFlyTrap.Models
{
    //TODO: Duplicate in TimeFlyTrap.WpfApp project
    public class AppSettings
    {
        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, string> PatternsWithTitleAlias { get; set; } = new Dictionary<string, string>();
    }
}