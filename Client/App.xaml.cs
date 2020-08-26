using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Client.Views;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IConfigurationRoot Configuration { get; private set; }
        public static IServiceProvider ServiceProveder { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ServiceProveder = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
            ServiceProveder.GetService<MainWindow>().Show();
        }

        private IServiceCollection ConfigureServices(IServiceCollection services) =>
            services.AddSingleton<IConfiguration>(Configuration)
            .AddSingleton<MainWindow>();
    }
}
