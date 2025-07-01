using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using WpfWeatherStations.Common.Interfaces;
using WpfWeatherStations.Services.Services;
using WpfWeatherStations.ViewModels.ViewModels;
using WpfWeatherStations.Views.Views;

namespace WpfWeatherStations
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Field
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Method
        public App()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<MainView>(provider => new MainView
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });

            serviceCollection.AddSingleton<MainViewModel>();
            serviceCollection.AddSingleton<IDispatcherService, DispatcherService>();
            serviceCollection.AddSingleton<IFileDialogService, FileDialogService>();
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetRequiredService<MainView>();
            mainWindow.Show();
            base.OnStartup(e);
        }
        #endregion
    }
}
