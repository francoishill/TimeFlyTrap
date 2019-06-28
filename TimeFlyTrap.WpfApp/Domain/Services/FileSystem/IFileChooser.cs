namespace TimeFlyTrap.WpfApp.Domain.Services.FileSystem
{
    public interface IFileChooser
    {
        bool Choose(FileType fileType);
        string ChosenFile { get; }
        bool DoesChosenFileExist { get; }
    }
}