using System;
using System.IO;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using TimeFlyTrap.Common;
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
            ConfigureLogging(services);

            var env = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");
            if (string.IsNullOrWhiteSpace(env))
            {
                // TODO this could fall back to an environment, rather than exception?
                throw new Exception("NETCORE_ENVIRONMENT env variable not set.");
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables("TimeFlyTrap_")
                .Build();

            services.AddSingleton<IMessenger>(Messenger.Default);

            services.ConfigureAndValidate<TokenProviderOptions>(configuration.GetSection("Tokens").Bind);
            services.AddTransient<ITokenProvider, TokenProvider>();
            services.AddTransient<IAppManager, AppManager>();
            services.AddTransient<IMainWindowProvider, MainWindowProvider>();

            services.AddTransient<IAppFileSystem, AppFileSystem>();
            services.AddTransient<IFileChooserFactory, FileChooserFactory>();

            services.AddTransient<INotifyIconViewModel, NotifyIconViewModel>();
            services.AddTransient<ISettingsViewModel, SettingsViewModel>();
            services.AddTransient<IReportViewModel, ReportViewModel>();
            services.AddTransient<IActiveWindowTrackerViewModel, ActiveWindowTrackerViewModel>();

            services.AddSingleton<ISettingsProvider, SettingsProvider>();
            services.AddSingleton<IActiveWindowsTracker, ActiveWindowsTracker>();
            services.ConfigureAndValidate<ApiUploaderOptions>(configuration.GetSection("Upload").Bind);
            services.AddSingleton<IApiUploader, ApiUploader>();
        }

        private static void ConfigureLogging(IServiceCollection services)
        {
            var logDir = Path.Combine(AppStateHelper.ApplicationLocalDataDirectory, "Logs");
            if (!Directory.Exists(logDir)) Directory.CreateDirectory(logDir);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // this is global, can filter on each file
                .WriteTo.RollingFile(Path.Combine(logDir, "log-{Date}-debug.log"), LogEventLevel.Debug, retainedFileCountLimit: 10)
                .WriteTo.RollingFile(Path.Combine(logDir, "log-{Date}-error.log"), LogEventLevel.Error, retainedFileCountLimit: 10)
                .CreateLogger();

            services.AddLogging(builder =>
                builder
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddDebug()
                    .AddConsole()
                    .AddSerilog(dispose: true)
            );
        }
    }
}