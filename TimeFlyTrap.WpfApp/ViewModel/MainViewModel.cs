using System;
using GalaSoft.MvvmLight;
using Microsoft.Extensions.DependencyInjection;
using TimeFlyTrap.WpfApp.Domain.ViewModels;

namespace TimeFlyTrap.WpfApp.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        public static IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                NotifyIcon = serviceProvider.GetRequiredService<INotifyIconViewModel>();
                Settings = serviceProvider.GetRequiredService<ISettingsViewModel>();
                Report = serviceProvider.GetRequiredService<IReportViewModel>();
                ActiveWindowTracker = serviceProvider.GetRequiredService<IActiveWindowTrackerViewModel>();

                ActiveWindowTracker.StartTracking();
            }
        }

        public INotifyIconViewModel NotifyIcon { get; }
        public ISettingsViewModel Settings { get; }
        public IReportViewModel Report { get; }
        public IActiveWindowTrackerViewModel ActiveWindowTracker { get; }
    }
}