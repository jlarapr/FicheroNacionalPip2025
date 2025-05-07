using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Presentation.BaseClass;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Models;
using FicheroNacionalPip.Presentation.Services;
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

    public ObservableCollection<MenuItemModel>? MenuItems { get; set; }
    public ObservableCollection<MenuItemModel>? RightMenuItems { get; set; }

    public MainBaseMainWindowViewModel(IViewService viewService, ILogger<BaseMainWindows> logger) 
        : base(logger) {
        _viewService = viewService;
        UserName = Environment.UserName;
        _logger.LogInformation("Iniciando MainBaseMainWindowViewModel para usuario: {UserName}", UserName);

        MyTitle = "Bienvenido al Fichero Nacional del Partido Independentista Puertorriqueño".ToUpper();
        var version = new FicheroNacionalPip.Business.Version();
        Title = $"{MyTitle}: {version}";
        _logger.LogInformation("Aplicación iniciada con versión: {Version}", version);

        ButtonCloseMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuVisibility = Visibility.Visible;
        ButtonCloseMenuIsEnabled = false;
        ButtonOpenMenuIsEnabled = true;

        CurrentView = _viewService.GetView("Home");
        _logger.LogInformation("Vista inicial cargada: Home");

        SetMenu();
        _logger.LogInformation("Menú principal inicializado");
    }

    private void SetMenu() {
        try {
            MenuItems = [
                new MenuItemModel { DisplayName = "Home", IconKind = PackIconKind.Home, Command = ButtonMenuItemCommand },
                new MenuItemModel {
                    DisplayName = "Search", IconKind = PackIconKind.Search,
                    SubItems = new ObservableCollection<MenuItemModel> {
                        new() { DisplayName = "Master Afiliados", IconKind = PackIconKind.Search, Command = ButtonMenuItemCommand, },
                        new() { DisplayName = "Master CEE", IconKind = PackIconKind.Search, Command = ButtonMenuItemCommand, }
                    }
                },
                new MenuItemModel {
                    DisplayName = "Reports", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand,
                    SubItems = new() {
                        new MenuItemModel { DisplayName = "Lista", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand },
                        new MenuItemModel { DisplayName = "Membretes", IconKind = PackIconKind.FileReport, Command = ButtonMenuItemCommand }
                    }
                },
                new MenuItemModel { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = ButtonMenuItemCommand }
            ];

            RightMenuItems = new ObservableCollection<MenuItemModel> {
                new() { DisplayName = "Admin", IconKind = PackIconKind.Account, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Settings", IconKind = PackIconKind.Settings, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Change Password", IconKind = PackIconKind.LockReset, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Help", IconKind = PackIconKind.HelpCircle, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Login", IconKind = PackIconKind.Login, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Logout", IconKind = PackIconKind.Logout, Command = ButtonMenuItemCommand },
                new() { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = ButtonMenuItemCommand }
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
        _logger.LogInformation("Ítem de menú seleccionado: {MenuItemName}", nameWindow);

        if (nameWindow == "Exit") {
            _logger.LogInformation("Iniciando proceso de cierre desde menú");
            OnClosing?.Invoke();
            return;
        }

        try {
            CurrentView = _viewService.GetView(nameWindow ?? "Home");
            _logger.LogInformation("Vista cambiada a: {ViewName}", nameWindow ?? "Home");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cambiar la vista a {ViewName}", nameWindow);
        }
    }
}