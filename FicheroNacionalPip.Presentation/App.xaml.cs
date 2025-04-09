using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Presentation.ViewModels;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;
using FicheroNacionalPip.Presentation.ViewModels.RightMenu;
using FicheroNacionalPip.Presentation.Views;
using FicheroNacionalPip.Presentation.Views.LeftMenu;
using FicheroNacionalPip.Presentation.Views.RightMenu;

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

                // Aquí registras todo
                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainBaseMainWindowViewModel>();

                // Right Menu
                services.AddSingleton<SettingWindow>();
                services.AddSingleton<SettingViewModel>();
                services.AddSingleton<AdminWindow>();
                services.AddSingleton<AdminViewModel>();
                services.AddSingleton<ChangePasswordWindow>();
                services.AddSingleton<ChangePasswordViewModel>();
                services.AddSingleton<HelpWindow>();
                services.AddSingleton<HelpViewModel>();
                services.AddSingleton<LoginWindow>();
                services.AddSingleton<LoginViewModel>();
                services.AddSingleton<LogoutWindow>();
                services.AddSingleton<LogoutViewModel>();

                // Left Menu
                services.AddSingleton<HomeWindow>();
                services.AddSingleton<HomeViewModel>();
                services.AddSingleton<ListaWindow>();
                services.AddSingleton<ListaViewModel>();
                services.AddSingleton<MasterAfiliadosWindow>();
                services.AddSingleton<MasterAfiliadosViewModel>();
                services.AddSingleton<MasterCeeWindow>();
                services.AddSingleton<MasterCeeViewModel>();
                services.AddSingleton<MembretesWindow>();
                services.AddSingleton<MembretesViewModel>();

                // Servicio abstracto
                //  services.AddSingleton<BaseMainWindows, BaseMainWindowsService>();
                services.AddSingleton<IViewService, ViewService>();
                services.AddSingleton(typeof(Lazy<>), typeof(LazyService<>));

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