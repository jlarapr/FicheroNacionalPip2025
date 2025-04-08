using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Presentation.ViewModels;
using FicheroNacionalPip.Presentation.Views;

namespace FicheroNacionalPip.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private static IHost? Host { get; set; }

    public App() {
        Host = Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder()
            .ConfigureServices((context, services) => {
                services.AddSingleton(typeof(Lazy<>), typeof(LazyService<>));

                // Aquí registras todo
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainBaseMainWindowViewModel>();
                services.AddSingleton<HomeWindow>();
                services.AddSingleton<HomeViewModel>();

                // Servicio abstracto
                //  services.AddSingleton<BaseMainWindows, BaseMainWindowsService>();
                services.AddSingleton<IViewService, ViewService>();

                // Logging opcional
                services.AddLogging(config => { config.AddConsole(); });
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e) {
        if (Host == null) {
            base.OnStartup(e);
            return;
        }

        await Host.StartAsync();

        // Crea la ventana principal desde el contenedor
        var mainWindow = Host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e) {
        if (Host == null) {
            base.OnExit(e);
            return;
        }

        await Host.StopAsync();
        Host.Dispose();
        base.OnExit(e);
    }
}