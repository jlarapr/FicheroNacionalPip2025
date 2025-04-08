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
            new MenuItemModel { DisplayName = "Home", IconKind = PackIconKind.Home, Command = YourItemCommand },
            new MenuItemModel { DisplayName = "Search", IconKind = PackIconKind.Search,
                SubItems = new ObservableCollection<MenuItemModel> {
                    new() { DisplayName = "Master Afiliados", IconKind = PackIconKind.Search, Command = YourItemCommand, },
                    new() { DisplayName = "Master CEE", IconKind = PackIconKind.Search, Command = YourItemCommand, }
                }
            },
            new MenuItemModel {
                DisplayName = "Reports", IconKind = PackIconKind.FileReport, Command = YourItemCommand,
                SubItems = new () {
                    new MenuItemModel { DisplayName = "Lista", IconKind = PackIconKind.FileReport, Command = YourItemCommand },
                    new MenuItemModel { DisplayName = "Membretes", IconKind = PackIconKind.FileReport, Command = YourItemCommand }
                }

            },
            new MenuItemModel { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = YourItemCommand }
        ];

        RightMenuItems = new ObservableCollection<MenuItemModel> {
            new() { DisplayName = "Admin", IconKind = PackIconKind.Account, Command = RightItemCommand },
            new() { DisplayName = "Settings", IconKind = PackIconKind.Settings, Command = RightItemCommand },
            new() { DisplayName = "Change Password", IconKind = PackIconKind.LockReset, Command = RightItemCommand },

            new() { DisplayName = "Help", IconKind = PackIconKind.HelpCircle, Command = RightItemCommand },
            new() { DisplayName = "Login", IconKind = PackIconKind.Login, Command = RightItemCommand },
            new() { DisplayName = "Logout", IconKind = PackIconKind.Logout, Command = RightItemCommand },
            new() { DisplayName = "Exit", IconKind = PackIconKind.ExitToApp, Command = RightItemCommand }
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
    private void YourItem(object parameter) {
        if (parameter is MenuItemModel clickedItem) {
            switch (clickedItem.DisplayName) {
                case "Home":
                    CurrentView = _viewService.GetView("Home");
                    break;
                case "Exit":
                    OnClosing?.Invoke();
                    break;
                default:
                    CurrentView = null;
                    break;
            }
        }
    }

    [RelayCommand]
    private void RightItem(object parameter) {
        if (parameter is MenuItemModel clickedItem) {
            switch (clickedItem.DisplayName) {
                case "Admin":
                case "Settings":
                case "Index Schema":
                case "Index Load":
                case "Index Delete":
                case "Change Password":
                case "Help":
                case "Login":
                case "Logout":
                    CurrentView = null;
                    break;
                case "Exit":
                    OnClosing?.Invoke();
                    break;
            }
        }
    }
}