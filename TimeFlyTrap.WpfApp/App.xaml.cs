using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.DependencyInjection;
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

            MainViewModel.ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMessenger>(Messenger.Default);

            services.AddTransient<IAppManager, AppManager>();
            services.AddTransient<IAppFilePathProvider, AppFilePathProvider>();
            services.AddTransient<IFileChooserFactory, FileChooserFactory>();

            services.AddTransient<INotifyIconViewModel, NotifyIconViewModel>();
            services.AddTransient<IReportViewModel, ReportViewModel>();
        }
    }
}