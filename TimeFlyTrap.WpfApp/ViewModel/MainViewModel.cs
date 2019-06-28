using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Domain.ViewModels;
using TimeFlyTrap.WpfApp.Events;

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

        private readonly IAppManager _appManager;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;

                _appManager = serviceProvider.GetRequiredService<IAppManager>();

                NotifyIcon = serviceProvider.GetRequiredService<INotifyIconViewModel>();
                Report = serviceProvider.GetRequiredService<IReportViewModel>();

                var messenger = serviceProvider.GetRequiredService<IMessenger>();
                messenger.Register<ExitApplicationEvent>(this, OnExitApplication);
            }
        }

        public INotifyIconViewModel NotifyIcon { get; }
        public IReportViewModel Report { get; }

        private void OnExitApplication(ExitApplicationEvent @event)
        {
            NotifyIcon.ShowNotifyIcon = false;
            _appManager.ShutDown();
        }
    }
}