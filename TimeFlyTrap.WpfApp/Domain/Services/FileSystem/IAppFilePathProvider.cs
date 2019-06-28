namespace TimeFlyTrap.WpfApp.Domain.Services.FileSystem
{
    public interface IAppFilePathProvider
    {
        string LocalAppData { get; }
        string SettingsFilePath { get; }
    }
}