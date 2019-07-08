using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using PlayAccumulateTimeFlyTrap.Models;
using PlayAccumulateTimeFlyTrap.Services;

namespace PlayAccumulateTimeFlyTrap.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IMainService mainService, ISettingsProvider settingsProvider)
        {
            var windowTimes = mainService.LoadWindowTimes();

            WindowTimes = windowTimes
                .GroupBy(x => FormatTitle(settingsProvider.Settings.PatternsWithTitleAlias, x.WindowTitle))
                .Select(x => new WindowTimesViewModel(
                    x.Key,
                    TimeSpan.FromSeconds(x.Sum(y => y.TotalDuration.TotalSeconds)),
                    TimeSpan.FromSeconds(x.Sum(y => y.IdleDuration.TotalSeconds)),
                    TimeSpan.FromSeconds(x.Sum(y => y.ActiveDuration.TotalSeconds)),
                    x.ToArray()))
                .OrderByDescending(x => x.ActiveDuration.TotalSeconds);

            ////if (IsInDesignMode)
            ////{
            ////    // Code runs in Blend --> create design time data.
            ////}
            ////else
            ////{
            ////    // Code runs "for real"
            ////}
        }

        public IEnumerable<WindowTimesViewModel> WindowTimes { get; set; }

        private string FormatTitle(Dictionary<string, string> patternsWithTitleAlias, string title)
        {
            var matches = patternsWithTitleAlias
                .Select(patternAndAlias => (Pattern: patternAndAlias.Key, Alias: patternAndAlias.Value, Match: new Regex(patternAndAlias.Key).Match(title)))
                .Where(x => x.Match.Success)
                .ToArray();

            if (matches.Length == 0)
            {
                return title;
            }

            if (matches.Length > 1)
            {
                //TODO: no logging
                Console.WriteLine($"ERROR: Multiple patterns match title '{title}', using first:\n{string.Join(Environment.NewLine, matches.Select(x => x.Pattern))}");
//                Log(
//                    LogLevel.Error,
//                    0,
//                    $"Multiple patterns match title '{title}', using first:\n{string.Join(Environment.NewLine, matches.Select(x => x.Pattern))}",
//                    null,
//                    (s, ex) => s);
            }

            var (_, @alias, firstMatch) = matches[0];

            var newTitle = @alias;

            for (var i = 0; i < firstMatch.Groups.Count; i++)
            {
                if (i == 0)
                {
                    continue; // The full match
                }

                newTitle = newTitle.Replace("{" + (i - 1) + "}", firstMatch.Groups[i].Value.Trim());
            }

            return newTitle;
        }
    }
}