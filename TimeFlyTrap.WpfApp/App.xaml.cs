using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TimeFlyTrap.Monitoring;
using TimeFlyTrap.WpfApp.Domain;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.Services.FileSystem;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Services;
using TimeFlyTrap.WpfApp.Services.FileSystem;
using TimeFlyTrap.WpfApp.ViewModel;

// ReSharper disable RedundantTypeArgumentsOfMethod

namespace TimeFlyTrap.WpfApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            TimeFlyTrap.WpfApp.MainWindow.ServiceProvider = serviceProvider;
            MainViewModel.ServiceProvider = serviceProvider;
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMessenger>(Messenger.Default);

            services.AddTransient<IAppManager, AppManager>();
            services.AddTransient<IMainWindowProvider, MainWindowProvider>();

            services.AddTransient<IAppFileSystem, AppFileSystem>();
            services.AddTransient<IFileChooserFactory, FileChooserFactory>();

            services.AddTransient<INotifyIconViewModel, NotifyIconViewModel>();
            services.AddTransient<ISettingsViewModel, SettingsViewModel>();
            services.AddTransient<IReportViewModel, ReportViewModel>();
            services.AddTransient<IActiveWindowTrackerViewModel, ActiveWindowTrackerViewModel>();
            services.AddTransient<ILogger>(s => s.GetRequiredService<IActiveWindowTrackerViewModel>());

            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<IActiveWindowsTracker, ActiveWindowsTracker>();
        }
    }
}