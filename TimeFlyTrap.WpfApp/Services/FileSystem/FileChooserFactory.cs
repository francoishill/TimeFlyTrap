using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class FileChooserFactory : IFileChooserFactory
    {
        private readonly IMainWindowProvider _mainWindowProvider;
        private readonly IAppFileSystem _appFileSystem;

        public FileChooserFactory(IMainWindowProvider mainWindowProvider, IAppFileSystem appFileSystem)
        {
            _mainWindowProvider = mainWindowProvider;
            _appFileSystem = appFileSystem;
        }

        public IFileChooser Create()
        {
            return new FileChooser(_mainWindowProvider, _appFileSystem);
        }
    }
}