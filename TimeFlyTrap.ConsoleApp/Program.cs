using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using TimeFlyTrap.Monitoring;

namespace TimeFlyTrap.ConsoleApp
{
    class Program
    {
        private static ActiveWindowsTracker _tracker;

        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedParameter.Local
        static void Main(string[] args)
        {
            _tracker = new ActiveWindowsTracker();

            _tracker.StartTicker(new TrackingListener());

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }

            // ReSharper disable once FunctionNeverReturns
        }

        private class TrackingListener : ITrackingListener
        {
            public void OnLastInfo(OnLastInfoEvent @event)
            {
                Console.WriteLine($"Startup: {@event.SystemStartupTime}, IdleDuration: {@event.IdleDuration}");
            }

            public void OnActiveWindowInfo(OnActiveWindowInfoEvent @event)
            {
                Console.WriteLine($"New title: {@event.Title}, Module: {@event.ModuleFilePath}");
            }
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_tracker.StopAndGetReport(out var report))
            {
                var dir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "TimeFlyTrap",
                    $"{DateTime.Now:yyyy-MM-dd}");

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var filePathWithoutExtension = Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd HH_mm_ss}");

                ActiveWindowsTracker.SaveReportsToJsonAndHtml(
                    new ObservableCollection<WindowTimes>(report.Values),
                    filePathWithoutExtension + ".json",
                    filePathWithoutExtension + ".html");
            }

            Console.WriteLine("Monitoring aborted, press any key to close");
            Console.ReadKey();
        }
    }
}