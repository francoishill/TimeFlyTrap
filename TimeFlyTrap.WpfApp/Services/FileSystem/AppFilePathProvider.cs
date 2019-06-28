using System;
using System.IO;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class AppFilePathProvider : IAppFilePathProvider
    {
        private const string APP_FOLDER_NAME = "TimeFlyTrap";
        
        public string LocalAppData()
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