using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using TimeFlyTrap.Monitoring;

namespace TimeFlyTrap.ConsoleApp
{
    class Program
    {
        private static WindowsMonitor _monitor;

        // ReSharper disable once ArrangeTypeMemberModifiers
        static void Main(string[] args)
        {
            _monitor = new WindowsMonitor(
                newTitle => { Console.WriteLine($"New title: {newTitle}"); },
                (systemStartupTime, idleDuration) => { Console.WriteLine($"Startup: {systemStartupTime}, IdleDuration: {idleDuration}"); });

            _monitor.StartTicker();

            Console.CancelKeyPress += ConsoleOnCancelKeyPress;

            while (true)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
            }
        }

        private static void ConsoleOnCancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            if (_monitor.StopAndGetReport(out var report))
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

                WindowsMonitor.SaveReportsToJsonAndHtml(
                    new ObservableCollection<WindowTimes>(report.Values),
                    filePathWithoutExtension + ".json",
                    filePathWithoutExtension + ".html");
            }
        }
    }
}