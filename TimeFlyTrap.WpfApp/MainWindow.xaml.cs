using System;
using System.ComponentModel;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Extensions.DependencyInjection;
using TimeFlyTrap.WpfApp.Domain.Services;
using TimeFlyTrap.WpfApp.Events;

namespace TimeFlyTrap.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public static IServiceProvider ServiceProvider { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var messenger = scope.ServiceProvider.GetRequiredService<IMessenger>();
                messenger.Send(new MainWindowLoadedEvent());
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            using (var scope = ServiceProvider.CreateScope())
            {
                var appManager = scope.ServiceProvider.GetRequiredService<IAppManager>();
                appManager.HideMainWindow();
            }
        }
    }
}