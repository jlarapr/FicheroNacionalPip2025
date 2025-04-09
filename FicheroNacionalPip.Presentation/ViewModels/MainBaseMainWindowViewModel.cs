using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Models;
using FicheroNacionalPip.Presentation.Services;
using MaterialDesignThemes.Wpf;

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

    public MainBaseMainWindowViewModel(IViewService viewService) {
        _viewService = viewService;
        UserName = Environment.UserName;

        MyTitle = "Bienvenido al Fichero Nacional del Partido Independentista Puertorriqueño".ToUpper();
        var version = new FicheroNacionalPip.Business.Version();
        Title = $"{MyTitle}: {version}";

        ButtonCloseMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuVisibility = Visibility.Visible;
        ButtonCloseMenuIsEnabled = false;
        ButtonOpenMenuIsEnabled = true;

        CurrentView = _viewService.GetView("Home");
        SetMenu();
    }

    private void SetMenu() {
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
    }

    [RelayCommand]
    private void BtnExit() {
        OnClosing?.Invoke();
    }

    [RelayCommand]
    private void ButtonCloseMenu() {
        ButtonCloseMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuVisibility = Visibility.Visible;
        ButtonOpenMenuIsEnabled = true;
        ButtonCloseMenuIsEnabled = false;
    }

    [RelayCommand]
    private void ButtonOpenMenu() {
        ButtonCloseMenuVisibility = Visibility.Visible;
        ButtonOpenMenuVisibility = Visibility.Collapsed;
        ButtonOpenMenuIsEnabled = false;
        ButtonCloseMenuIsEnabled = true;
    }

    [RelayCommand]
    private void ButtonMenuItem(object parameter) {
        if (parameter is not MenuItemModel clickedItem) return;

        string? nameWindow = clickedItem.DisplayName;

        if (nameWindow == "Exit") {
            OnClosing?.Invoke();
            return;
        }

        CurrentView = _viewService.GetView(nameWindow ?? "Home");
    }
}