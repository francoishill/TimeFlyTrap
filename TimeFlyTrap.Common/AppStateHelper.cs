using System;
using System.IO;

namespace TimeFlyTrap.Common
{
    public class AppStateHelper
    {
        private const string APP_FOLDER_NAME = "TimeFlyTrap";

        public static string ApplicationLocalDataDirectory
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), APP_FOLDER_NAME);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                return dir;
            }
        }
    }
}