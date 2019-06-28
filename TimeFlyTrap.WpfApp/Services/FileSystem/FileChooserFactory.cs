using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class FileChooserFactory : IFileChooserFactory
    {
        private readonly IAppFilePathProvider _appFilePathProvider;

        public FileChooserFactory(IAppFilePathProvider appFilePathProvider)
        {
            _appFilePathProvider = appFilePathProvider;
        }

        public IFileChooser Create()
        {
            return new FileChooser(_appFilePathProvider);
        }
    }
}