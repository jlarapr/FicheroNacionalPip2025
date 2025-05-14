using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Presentation.BaseClass;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Models;
using FicheroNacionalPip.Presentation.Services;
using FicheroNacionalPip.Presentation.ViewModels.RightMenu;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace FicheroNacionalPip.Presentation.ViewModels;

public partial class MainBaseMainWindowViewModel : BaseMainWindowsService {
    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _myTitle = string.Empty;
    [ObservableProperty] private string _title = string.Empty;
    [ObservableProperty] private bool _buttonOpenMenuIsEnabled;
    [ObservableProperty] private bool _buttonCloseMenuIsEnabled;
    [ObservableProperty] private Visibility _buttonOpenMenuVisibility;
    [ObservableProperty] private Visibility _buttonCloseMenuVisibility;
    [ObservableProperty] private FrameworkElement? _currentView;

    private readonly IViewService _viewService;
    private readonly LoginViewModel _loginViewModel;
    private readonly IAuthenticationService _authService;

    public ObservableCollection<MenuItemModel>? MenuItems { get; set; }
    public ObservableCollection<MenuItemModel>? RightMenuItems { get; set; }

    public MainBaseMainWindowViewModel(IViewService viewService, ILogger<BaseMainWindows> logger, LoginViewModel loginViewModel,
        IAuthenticationService authService) : base(logger) {
        _viewService = viewService;
        _loginViewModel = loginViewModel;
        _authService = authService;

        UserName = Environment.UserName;
        _logger.LogInformation("Iniciando MainBaseMainWindowViewModel para usuario: {UserName}", UserName);

        // Suscribirse a eventos
        _loginViewModel.LoginSuccessful += OnLoginSuccessful;
        _logger.LogDebug("Suscrito a eventos de autenticación");

        MyTitle = "Bienvenido al Fichero Nacional del Partido Independentista Puertorriqueño".ToUpper();
        var version = new FicheroNacionalPip.Business.Version();
        Title = $"{MyTitle}: {version}";
        _logger.LogInformation("Aplicación iniciada con versión: {Version}", version);

        ButtonCloseMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuVisibility = Visibility.Visible;
        ButtonCloseMenuIsEnabled = false;
        ButtonOpenMenuIsEnabled = true;

        CurrentView = _viewService.GetView("Login");
        _logger.LogInformation("Vista inicial cargada: Login");

        SetMenu();

        _logger.LogInformation("Menú principal inicializado");
    }

    // Método que maneja el evento de login exitoso
    private void OnLoginSuccessful(UserAuthInfo userInfo) {
        _logger.LogInformation("Login exitoso detectado para usuario: {UserName}", userInfo.UserName);

        // Actualizar el nombre de usuario en la UI
        UserName = userInfo.UserName;

        // Actualizar los permisos de menú basados en las áreas de acceso del usuario
        UpdateMenuPermissions(userInfo.AreasDeAcceso);

        // Cambiar a la vista Home
        CurrentView = _viewService.GetView("Home");
        _logger.LogInformation("Vista cambiada a Home después del login exitoso");
    }

    private void SetMenu() {
        try {
            MenuItems = [
                new MenuItemModel { DisplayName = "Home", IconKind = PackIconKind.Home, Command = ButtonMenuItemCommand },
                new MenuItemModel {
                    DisplayName = "Search", IconKind = PackIconKind.Search,
                    SubItems = new ObservableCollection<MenuItemModel> {
                        new() { DisplayName = "Master Afiliados", IconKind = PackIconKind.Search, Command = ButtonMenuItemCommand, IsEnabled = false },
                        new() { DisplayName = "Master CEE", IconKind = PackIconKind.Search, Command = ButtonMenuItemCommand, IsEnabled = false }
                    }
                },
                new MenuItemModel {
                    DisplayName = "Reports", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand,
                    SubItems = new() {
                        new MenuItemModel { DisplayName = "Lista", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand, IsEnabled = false },
                        new MenuItemModel { DisplayName = "Membretes", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand, IsEnabled = false }
                    }
                },
                new MenuItemModel { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = ButtonMenuItemCommand, IsEnabled = true }
            ];

            RightMenuItems = new ObservableCollection<MenuItemModel> {
                new() { DisplayName = "Admin", IconKind = PackIconKind.Account, Command = ButtonMenuItemCommand, IsEnabled = false },
                new() { DisplayName = "Settings", IconKind = PackIconKind.Settings, Command = ButtonMenuItemCommand, IsEnabled = false },
                new() { DisplayName = "Change Password", IconKind = PackIconKind.LockReset, Command = ButtonMenuItemCommand, IsEnabled = false },
                new() { DisplayName = "Help", IconKind = PackIconKind.HelpCircle, Command = ButtonMenuItemCommand, IsEnabled = false },
                new() { DisplayName = "Login", IconKind = PackIconKind.Login, Command = ButtonMenuItemCommand, IsEnabled = true },
                new() { DisplayName = "Logout", IconKind = PackIconKind.Logout, Command = ButtonMenuItemCommand, IsEnabled = false },
                new() { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = ButtonMenuItemCommand, IsEnabled = true }
            };

            _logger.LogDebug("Menú principal y menú derecho configurados exitosamente");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al configurar los menús");
            throw;
        }
    }

    [RelayCommand]
    private void BtnExit() {
        _logger.LogInformation("Botón de salida presionado");
        OnClosing?.Invoke();
    }

    [RelayCommand]
    private void ButtonCloseMenu() {
        _logger.LogDebug("Cerrando menú");
        ButtonCloseMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuVisibility = Visibility.Visible;
        ButtonOpenMenuIsEnabled = true;
        ButtonCloseMenuIsEnabled = false;
        OnToggleMenu(false);
    }

    [RelayCommand]
    private void ButtonOpenMenu() {
        _logger.LogDebug("Abriendo menú");
        ButtonCloseMenuVisibility = Visibility.Visible;
        ButtonOpenMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuIsEnabled = false;
        ButtonCloseMenuIsEnabled = true;
        OnToggleMenu(true);
    }

    [RelayCommand]
    private void ButtonMenuItem(object parameter) {
        if (parameter is not MenuItemModel clickedItem) {
            _logger.LogWarning("Se intentó hacer clic en un ítem de menú inválido");
            return;
        }

        string? nameWindow = clickedItem.DisplayName;
        _logger.LogInformation("Ítem de menú seleccionado: {MenuItemName}, IsEnabled: {IsEnabled}", nameWindow, clickedItem.IsEnabled);

        // Verificar si el ítem está habilitado
        if (!clickedItem.IsEnabled) {
            _logger.LogWarning("Se intentó hacer clic en un ítem de menú deshabilitado: {MenuItemName}", nameWindow);
            return;
        }

        switch (nameWindow) {
            // Manejar el logout directamente
            case "Logout":
                _logger.LogInformation("Iniciando proceso de logout desde menú");
                PerformLogout();
                return;
            case "Login":
                // Resetear el ViewModel de login para limpiar todos los campos
                _loginViewModel.Reset();
                break;
            case "Exit":
                _logger.LogInformation("Iniciando proceso de cierre desde menú");
                OnClosing?.Invoke();
                return;
        }

        try {
            _logger.LogInformation("Cambiando vista a: {ViewName}", nameWindow ?? "Home");
            CurrentView = _viewService.GetView(nameWindow ?? "Home");
            _logger.LogInformation("Vista cambiada exitosamente a: {ViewName}", nameWindow ?? "Home");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cambiar la vista a {ViewName}", nameWindow);
        }
    }

    /// <summary>
    /// Handles the process of user logout by performing the following actions:
    /// logging out the current user, resetting the username to the machine's
    /// environment username, reinitializing the menu permissions, and switching
    /// the application view to the login view. Logs information and errors during
    /// the process.
    /// </summary>
    private void PerformLogout() {
        try {
            _logger.LogInformation("Iniciando cierre de sesión");

            // Obtener el usuario actual antes del logout para el log
            UserAuthInfo? currentUser = _authService.GetCurrentUser();

            // Realizar el logout
            _authService.Logout();

            _logger.LogInformation("Sesión cerrada correctamente para el usuario: {UserName}",
                currentUser?.UserName ?? "Desconocido");

            // Restablecer el nombre de usuario
            UserName = Environment.UserName;

            // Restablecer menu
            InitializeMenuPermissions();

            // Resetear el ViewModel de login para limpiar todos los campos
            _loginViewModel.Reset();

            // Cambiar a la vista Login
            CurrentView = _viewService.GetView("Login");
            _logger.LogInformation("Vista cambiada a Login después del logout");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cerrar la sesión");
        }
    }

    /// <summary>
    /// Actualiza los permisos de los elementos del menú según las áreas de acceso del usuario.
    /// </summary>
    /// <param name="areasDeAcceso">Áreas de acceso del usuario, separadas por comas</param>
    private void UpdateMenuPermissions(string areasDeAcceso) {
        _logger.LogInformation("Actualizando permisos de menú para áreas de acceso: {AreasDeAcceso}", areasDeAcceso);

        // Si el usuario tiene acceso a todas las áreas
        bool hasFullAccess = areasDeAcceso.Contains("ALL", StringComparison.OrdinalIgnoreCase);

        if (hasFullAccess) {
            if (MenuItems is not null) {
                foreach (MenuItemModel item in MenuItems) {
                    item.IsEnabled = true;
                    if (item.SubItems is null) continue;
                    foreach (MenuItemModel itemSubItem in item.SubItems) {
                        itemSubItem.IsEnabled = true;
                    }
                }
            }

            if (RightMenuItems is not null) {
                foreach (MenuItemModel item in RightMenuItems) {
                    item.IsEnabled = item.DisplayName != "Login";
                }
            }
        }

        // Habilitar/deshabilitar elementos del menú izquierdo
        // if (MenuItems != null)
        // {
        //     // Habilitar Home para todos los usuarios autenticados
        //     MenuItemModel? homeItem = MenuItems.FirstOrDefault(m => m.DisplayName == "Home");
        //     if (homeItem != null) homeItem.IsEnabled = true;
        //
        //     // Configurar permisos para el menú Search
        //     MenuItemModel? searchItem = MenuItems.FirstOrDefault(m => m.DisplayName == "Search");
        //     if (searchItem != null && searchItem.SubItems != null)
        //     {
        //         MenuItemModel? masterAfiliadosItem = searchItem.SubItems.FirstOrDefault(m => m.DisplayName == "Master Afiliados");
        //         if (masterAfiliadosItem != null)
        //         {
        //             masterAfiliadosItem.IsEnabled = hasFullAccess || areasDeAcceso.Contains("AFILIADOS", StringComparison.OrdinalIgnoreCase);
        //         }
        //
        //         MenuItemModel? masterCeeItem = searchItem.SubItems.FirstOrDefault(m => m.DisplayName == "Master CEE");
        //         if (masterCeeItem != null)
        //         {
        //             masterCeeItem.IsEnabled = hasFullAccess ||
        //                                      areasDeAcceso.Contains("CEE", StringComparison.OrdinalIgnoreCase);
        //         }
        //
        //         // Habilitar el menú principal si al menos un submenú está habilitado
        //         searchItem.IsEnabled = searchItem.SubItems.Any(s => s.IsEnabled);
        //     }
        //
        //     // Configurar permisos para el menú Reports
        //     MenuItemModel? reportsItem = MenuItems.FirstOrDefault(m => m.DisplayName == "Reports");
        //     if (reportsItem != null && reportsItem.SubItems != null)
        //     {
        //         MenuItemModel? listaItem = reportsItem.SubItems.FirstOrDefault(m => m.DisplayName == "Lista");
        //         if (listaItem != null)
        //         {
        //             listaItem.IsEnabled = hasFullAccess ||
        //                                  areasDeAcceso.Contains("REPORTES", StringComparison.OrdinalIgnoreCase);
        //         }
        //
        //         MenuItemModel? membretesItem = reportsItem.SubItems.FirstOrDefault(m => m.DisplayName == "Membretes");
        //         if (membretesItem != null)
        //         {
        //             membretesItem.IsEnabled = hasFullAccess ||
        //                                      areasDeAcceso.Contains("REPORTES", StringComparison.OrdinalIgnoreCase);
        //         }
        //
        //         // Habilitar el menú principal si al menos un submenú está habilitado
        //         reportsItem.IsEnabled = reportsItem.SubItems.Any(s => s.IsEnabled);
        //     }
        // }
        //
        // // Habilitar/deshabilitar elementos del menú derecho
        // if (RightMenuItems != null)
        // {
        //     // Admin solo para usuarios con acceso completo
        //     MenuItemModel? adminItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Admin");
        //     if (adminItem != null) adminItem.IsEnabled = hasFullAccess;
        //
        //     // Settings para usuarios con permisos específicos
        //     MenuItemModel? settingsItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Settings");
        //     if (settingsItem != null)
        //         settingsItem.IsEnabled = hasFullAccess ||
        //                                 areasDeAcceso.Contains("SETTINGS", StringComparison.OrdinalIgnoreCase);
        //
        //     // Change Password disponible para todos los usuarios autenticados
        //     MenuItemModel? changePasswordItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Change Password");
        //     if (changePasswordItem != null) changePasswordItem.IsEnabled = true;
        //
        //     // Help disponible para todos los usuarios autenticados
        //     MenuItemModel? helpItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Help");
        //     if (helpItem != null) helpItem.IsEnabled = true;
        //
        //     // Login deshabilitado cuando el usuario está autenticado
        //     MenuItemModel? loginItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Login");
        //     if (loginItem != null) loginItem.IsEnabled = false;
        //
        //     // Logout habilitado cuando el usuario está autenticado
        //     MenuItemModel? logoutItem = RightMenuItems.FirstOrDefault(m => m.DisplayName == "Logout");
        //     if (logoutItem != null) logoutItem.IsEnabled = true;
        // }

        _logger.LogDebug("Permisos de menú actualizados correctamente");
    }

    /// <summary>
    /// Obtiene la vista de cambio de contraseña.
    /// </summary>
    public FrameworkElement GetChangePasswordView() {
        return _viewService.GetView("Change Password");
    }

    /// <summary>
    /// Obtiene la vista principal (Home).
    /// </summary>
    public FrameworkElement GetHomeView() {
        return _viewService.GetView("Home");
    }

    // Método para configurar los permisos iniciales del menú
    private void InitializeMenuPermissions() {
        _logger.LogDebug("Configurando permisos iniciales de menú");

        // Menú izquierdo
        if (MenuItems != null) {
            // Solo Home y Exit habilitados inicialmente
            foreach (MenuItemModel item in MenuItems) {
                // bool isEnabled = item.DisplayName == "Home" || item.DisplayName == "Exit";
                item.IsEnabled = true;

                // Submenús deshabilitados inicialmente
                if (item.SubItems == null) continue;
                foreach (MenuItemModel subItem in item.SubItems) {
                    subItem.IsEnabled = false;
                }
            }
        }

        // Menú derecho
        if (RightMenuItems != null) {
            foreach (var item in RightMenuItems) {
                // Solo Login y Exit habilitados inicialmente
                bool isEnabled = item.DisplayName == "Login" || item.DisplayName == "Exit";
                item.IsEnabled = isEnabled;
            }
        }

        _logger.LogDebug("Permisos iniciales de menú configurados correctamente");
    }
}