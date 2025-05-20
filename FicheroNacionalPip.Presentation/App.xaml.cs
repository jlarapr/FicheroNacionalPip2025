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
using Microsoft.Extensions.Configuration;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Services;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using Microsoft.EntityFrameworkCore;
using FicheroNacionalPip.Data.Configuration;

namespace FicheroNacionalPip.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
    private IConfiguration _configuration;
    private ServiceProvider _serviceProvider;

    public App() {
        try {
            Result<bool, string> initResult = InitializeLogger();
            if (initResult.IsFailure) {
                ShowFatalError($"Error al inicializar el sistema de logs: {initResult.GetErrorOrDefault() ?? "Error desconocido"}");
                return;
            }

            if (!ConfigureServices()) {
                return; // El error ya ha sido mostrado en ConfigureServices
            }
        }
        catch (Exception ex) {
            ShowFatalError($"Error fatal durante la inicialización de la aplicación: {ex.Message}");
        }
    }

    private void ShowFatalError(string message) {
        MessageBox.Show(message, "Error Fatal", MessageBoxButton.OK, MessageBoxImage.Error);
        Current.Shutdown();
    }

    private Result<bool, string> InitializeLogger() {
        try {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "app.log");
            string? logDirectory = Path.GetDirectoryName(logPath);

            if (string.IsNullOrEmpty(logDirectory)) {
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
        catch (Exception ex) {
            return Result<bool, string>.Fail($"Error al inicializar el sistema de logs: {ex.Message}");
        }
    }

    private bool ConfigureServices() {
        try {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var environment = GetEnvironment();

            // Verificar la existencia de los archivos de configuración
            var mainSettingsPath = Path.Combine(basePath, "appsettings.json");
            var envSettingsPath = Path.Combine(basePath, $"appsettings.{environment}.json");

            if (!File.Exists(mainSettingsPath)) {
                throw new FileNotFoundException(
                    $"No se encontró el archivo de configuración principal en: {mainSettingsPath}");
            }

            // Configurar la configuración
            var builder = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            // Solo agregar el archivo de ambiente si existe
            if (File.Exists(envSettingsPath)) {
                builder.AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);
            }
            else {
                Log.Warning("No se encontró el archivo de configuración para el ambiente {Environment} en: {Path}",
                    environment, envSettingsPath);
            }

            builder.AddEnvironmentVariables();
            _configuration = builder.Build();

            // Verificar la sección DatabaseConfig
            var dbConfig = _configuration.GetSection("DatabaseConfig");
            if (!dbConfig.Exists()) {
                throw new InvalidOperationException("La sección 'DatabaseConfig' no existe en la configuración.");
            }

            // Configurar la inyección de dependencias
            var services = new ServiceCollection();

            // Configurar logging
            services.AddLogging(builder => {
                builder.ClearProviders();
                builder.AddSerilog(dispose: true);
            });

            // Registrar servicios
            services.AddSingleton(_configuration);
            services.AddSingleton<IDbConfigurationService, DbConfigurationService>();
            services.AddSingleton<ApplicationInitializer>();

            // Registrar DbContext
            services.AddDbContextFactory<ApplicationDbContext>(options => {
                try {
                    // Usar DatabaseConfiguration para manejar la desencriptación
                    var databaseConfig = new DatabaseConfiguration {
                        Server = dbConfig["Server"] ?? "localhost",
                        Database = dbConfig["Database"] ?? "FicheroNacionalPip",
                        IntegratedSecurity = bool.Parse(dbConfig["IntegratedSecurity"] ?? "true"),
                        TrustServerCertificate = bool.Parse(dbConfig["TrustServerCertificate"] ?? "true")
                    };

                    if (!databaseConfig.IntegratedSecurity) {
                        databaseConfig.Username = dbConfig["Username"] ?? "";
                        databaseConfig.Password = dbConfig["Password"] ?? "";
                    }

                    var connectionString = databaseConfig.GetConnectionString();
                    Log.Information("Configurando DbContext con cadena de conexión para servidor: {Server}, base de datos: {Database}",
                        databaseConfig.Server, databaseConfig.Database);

                    options.UseSqlServer(connectionString);
                }
                catch (Exception ex) {
                    Log.Error(ex, "Error al configurar la conexión a la base de datos");
                    throw;
                }
            });

            // Registrar servicio de autenticación
            services.AddSingleton<IAuthenticationService, AuthenticationService>();

            // Registrar servicio de política de contraseñas
            services.AddScoped<IPasswordPolicyService, PasswordPolicyService>();

            // Registrar servicios de la UI
            services.AddSingleton<IViewService, ViewService>();
            services.AddSingleton<IDialogService, DialogService>();
            services.AddSingleton(typeof(Lazy<>), typeof(LazyService<>));

            // Registrar MainWindow y su ViewModel
            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainBaseMainWindowViewModel>();
            services.AddSingleton<BaseMainWindowsService>();

            // Registrar ventanas del menú derecho
            
            services.AddSingleton<AdminWindow>();
            services.AddSingleton<AdminViewModel>();
            services.AddSingleton<ChangePasswordWindow>();
            services.AddSingleton<ChangePasswordViewModel>();
            services.AddSingleton<HelpWindow>();
            services.AddSingleton<HelpViewModel>();
            services.AddSingleton<LoginWindow>();
            services.AddSingleton<LoginViewModel>();

            // Registrar SettingViewModel y SettingWindow como transitorio para obtener nueva instancia cada vez
            services.AddTransient<SettingViewModel>();
            services.AddTransient<SettingWindow>();
            
            // Registrar ventanas del menú izquierdo
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

            // Construir el contenedor de servicios
            _serviceProvider = services.BuildServiceProvider();

            Log.Information("Servicios configurados correctamente");
            return true;
        }
        catch (Exception ex) {
            var errorMessage = "Error al configurar los servicios de la aplicación";
            if (Log.Logger != null) {
                Log.Error(ex, errorMessage);
            }

            ShowFatalError($"{errorMessage}: {ex.Message}");
            return false;
        }
    }

    protected override void OnStartup(StartupEventArgs e) {
        try {
            base.OnStartup(e);
            Log.Information("Iniciando aplicación...");

            if (_serviceProvider == null) {
                throw new InvalidOperationException("El proveedor de servicios no está inicializado.");
            }

            var initResult = InitializeApplication();
            if (initResult.IsFailure) {
                var errorMessage = initResult.GetErrorOrDefault() ?? "Error desconocido durante la inicialización";
                Log.Error("Error de inicialización: {Error}", errorMessage);
                ShowFatalError(errorMessage);
                return;
            }

            Log.Information("Aplicación iniciada correctamente");
        }
        catch (Exception ex) {
            Log.Fatal(ex, "Error fatal durante el inicio de la aplicación");
            ShowFatalError($"Error fatal al iniciar la aplicación: {ex.Message}");
        }
    }

    private Result<bool, string> InitializeApplication() {
        try {
            if (_serviceProvider == null) {
                return Result<bool, string>.Fail("El proveedor de servicios no está inicializado.");
            }

            var initializer = _serviceProvider.GetRequiredService<ApplicationInitializer>();
            var result = initializer.Initialize();

            if (result.IsFailure) {
                var errorMessage = result.GetErrorOrDefault() ?? "Error desconocido en la inicialización";
                return Result<bool, string>.Fail(errorMessage);
            }

            // Inicializar y mostrar la ventana principal
            MainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex) {
            return Result<bool, string>.Fail($"Error inesperado al inicializar la aplicación: {ex.Message}");
        }
    }

    private string GetEnvironment() {
#if DEBUG
        return "Development";
#else
            return "Production";
#endif
    }

    protected override void OnExit(ExitEventArgs e) {
        try {
            Log.Information("Aplicación cerrándose normalmente");
            Log.CloseAndFlush();

            if (_serviceProvider != null) {
                _serviceProvider.Dispose();
            }
        }
        catch (Exception ex) {
            Log.Fatal(ex, "Error durante el cierre de la aplicación");
            MessageBox.Show($"Error durante el cierre de la aplicación: {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally {
            base.OnExit(e);
        }
    }
}