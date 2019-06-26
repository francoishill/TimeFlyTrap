using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace TimeFlyTrap.Monitoring
{
    public class Win32
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo plii);

        internal struct LastInputInfo
        {
            public uint cbSize;
            public uint dwTime;
        }

        public static bool GetLastInputInfo(out DateTime systemStartupTime, out TimeSpan idleTime)
        {
            systemStartupTime = DateTime.MinValue;
            idleTime = TimeSpan.Zero;

            // Get the system uptime
            var ticksSinceSystemStarted = Environment.TickCount;
            // The tick at which the last input was recorded
            var LastInputTicks = 0;
            // The number of ticks that passed since last input
            var IdleTicks = 0;

            // Set the struct
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.cbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.dwTime = 0;

            // If we have a value from the function
            if (!GetLastInputInfo(ref lastInputInfo))
            {
                return false;
            }

            // Get the number of ticks at the point when the last activity was seen
            LastInputTicks = (int) lastInputInfo.dwTime;
            // Number of idle ticks = system uptime ticks - number of ticks at last input
            IdleTicks = ticksSinceSystemStarted - LastInputTicks;

            systemStartupTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(ticksSinceSystemStarted));
            idleTime = TimeSpan.FromMilliseconds(IdleTicks);

            return true;
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        public static string GetProcessOfWindowHandle(IntPtr windowHandle)
        {
            uint pid;
            GetWindowThreadProcessId(windowHandle, out pid);
            try
            {
                var p = Process.GetProcessById((int)pid);
                return p.MainModule?.FileName;
            }
            catch(Exception exception)
            {
                Console.WriteLine($"Failed to GetProcessOfWindowHandle, exception: " + exception.Message);
                return null;
            }
        }
    }
}