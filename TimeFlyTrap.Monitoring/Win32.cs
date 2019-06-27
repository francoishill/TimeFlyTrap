using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TimeFlyTrap.Monitoring
{
    public static class Win32
    {
        [DllImport("User32.dll")]
        private static extern bool GetLastInputInfo(ref LastInputInfo plii);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);
        
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint GetWindowModuleFileName(IntPtr hwnd, StringBuilder lpszFileName, uint cchFileNameMax);

        private struct LastInputInfo
        {
            // ReSharper disable once NotAccessedField.Local
            public uint CbSize;
            public uint DwTime;
        }

        public static bool GetLastInputInfo(out DateTime systemStartupTime, out TimeSpan idleTime)
        {
            systemStartupTime = DateTime.MinValue;
            idleTime = TimeSpan.Zero;

            // Get the system uptime
            var ticksSinceSystemStarted = Environment.TickCount;
            // The tick at which the last input was recorded
            int lastInputTicks;
            // The number of ticks that passed since last input
            int idleTicks;

            // Set the struct
            var lastInputInfo = new LastInputInfo();
            lastInputInfo.CbSize = (uint) Marshal.SizeOf(lastInputInfo);
            lastInputInfo.DwTime = 0;

            // If we have a value from the function
            if (!GetLastInputInfo(ref lastInputInfo))
            {
                return false;
            }

            // Get the number of ticks at the point when the last activity was seen
            lastInputTicks = (int) lastInputInfo.DwTime;
            // Number of idle ticks = system uptime ticks - number of ticks at last input
            idleTicks = ticksSinceSystemStarted - lastInputTicks;

            systemStartupTime = DateTime.Now.Subtract(TimeSpan.FromMilliseconds(ticksSinceSystemStarted));
            idleTime = TimeSpan.FromMilliseconds(idleTicks);

            return true;
        }

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