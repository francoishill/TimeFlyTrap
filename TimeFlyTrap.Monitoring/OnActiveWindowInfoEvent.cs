namespace TimeFlyTrap.Monitoring
{
    public class OnActiveWindowInfoEvent
    {
        public string Title { get; }
        public string ModuleFilePath { get; }

        public OnActiveWindowInfoEvent(string title, string moduleFilePath)
        {
            Title = title;
            ModuleFilePath = moduleFilePath;
        }
    }
}