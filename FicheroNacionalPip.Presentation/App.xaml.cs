using System.IO;
using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Presentation.ViewModels;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;
using FicheroNacionalPip.Presentation.ViewModels.RightMenu;
using FicheroNacionalPip.Presentation.Views;
using FicheroNacionalPip.Presentation.Views.LeftMenu;
using FicheroNacionalPip.Presentation.Views.RightMenu;
using Serilog;
using FicheroNacionalPip.Business.Common;

namespace FicheroNacionalPip.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private readonly ServiceProvider _serviceProvider;

    public App() {
        Result<bool, string> initResult = InitializeLogger();
        if (initResult.IsFailure)
        {
            MessageBox.Show($"Error al inicializar el sistema de logs: {initResult.GetErrorOrDefault()}", 
                "Error de Inicialización", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
            return;
        }

        var services = new ServiceCollection();
        ConfigureServices(services);
        _serviceProvider = services.BuildServiceProvider();
    }

    private Result<bool, string> InitializeLogger()
    {
        try
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
            string? logDirectory = Path.GetDirectoryName(logPath);
            
            if (string.IsNullOrEmpty(logDirectory))
            {
                return Result<bool, string>.Fail("No se pudo determinar el directorio de logs");
            }

            Directory.CreateDirectory(logDirectory);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("Sistema de logs inicializado correctamente en: {LogPath}", logPath);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            return Result<bool, string>.Fail($"Error al inicializar el sistema de logs: {ex.Message}");
        }
    }

    private void ConfigureServices(IServiceCollection services) {
        // Configurar logging
        services.AddLogging(builder => {
            builder.ClearProviders();
            builder.AddSerilog(dispose: true);
        });

        // Registrar servicios
        services.AddSingleton<IViewService, ViewService>();
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainBaseMainWindowViewModel>();
        services.AddSingleton<BaseMainWindowsService>();

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

        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton(typeof(Lazy<>), typeof(LazyService<>));
    }

    protected override void OnStartup(StartupEventArgs e) {
        try
        {
            base.OnStartup(e);
            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            Log.Information("Aplicación iniciada correctamente");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error fatal durante el inicio de la aplicación");
            MessageBox.Show($"Error al iniciar la aplicación: {ex.Message}", 
                "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
            Current.Shutdown();
        }
    }

    protected override void OnExit(ExitEventArgs e) {
        try
        {
            Log.Information("Aplicación cerrándose normalmente");
            Log.CloseAndFlush();
            _serviceProvider.Dispose();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Error durante el cierre de la aplicación");
        }
        finally
        {
            base.OnExit(e);
        }
    }
}