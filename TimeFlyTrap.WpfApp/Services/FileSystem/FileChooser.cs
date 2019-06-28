using System;
using System.IO;
using Microsoft.Win32;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;

namespace TimeFlyTrap.WpfApp.Services.FileSystem
{
    public class FileChooser : IFileChooser
    {
        private readonly IMainWindowProvider _mainWindowProvider;
        private readonly IAppFileSystem _appFileSystem;

        public FileChooser(IMainWindowProvider mainWindowProvider, IAppFileSystem appFileSystem)
        {
            _mainWindowProvider = mainWindowProvider;
            _appFileSystem = appFileSystem;
        }

        public bool Choose(FileType fileType)
        {
            var openFileDialog = new OpenFileDialog
            {
                InitialDirectory = _appFileSystem.LocalAppData,
                Filter = FileFilterFromType(fileType),
                CheckFileExists = true,
                CheckPathExists = true,
            };

            ChosenFile = null;

            var result = openFileDialog.ShowDialog(_mainWindowProvider.Window);
            if (result == true)
            {
                ChosenFile = openFileDialog.FileName;
            }
            return result == true;
        }

        private static string FileFilterFromType(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.ReportJson: return "json files (*.json)|*.json|All files (*.*)|*.*";

                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        public bool DoesChosenFileExist => File.Exists(ChosenFile);
        public string ChosenFile { get; private set; }
    }
}