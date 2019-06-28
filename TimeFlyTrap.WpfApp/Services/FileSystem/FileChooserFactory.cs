using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class FileChooserFactory : IFileChooserFactory
    {
        private readonly IMainWindowProvider _mainWindowProvider;
        private readonly IAppFilePathProvider _appFilePathProvider;

        public FileChooserFactory(IMainWindowProvider mainWindowProvider, IAppFilePathProvider appFilePathProvider)
        {
            _mainWindowProvider = mainWindowProvider;
            _appFilePathProvider = appFilePathProvider;
        }

        public IFileChooser Create()
        {
            return new FileChooser(_mainWindowProvider, _appFilePathProvider);
        }
    }
}