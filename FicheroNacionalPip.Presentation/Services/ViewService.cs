using System.Windows;
using System.Windows.Controls;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Views.LeftMenu;
using FicheroNacionalPip.Presentation.Views.RightMenu;
using Microsoft.Extensions.DependencyInjection;

namespace FicheroNacionalPip.Presentation.Services;

public class ViewService : IViewService
{
    private readonly IServiceProvider _serviceProvider;

    public ViewService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public FrameworkElement GetView(string viewName)
    {
        return viewName switch
        {
            // Left Menu
            "Home" => CreateFrameworkElement<HomeWindow>(),
            "Master Afiliados" => CreateFrameworkElement<MasterAfiliadosWindow>(),
            "Master CEE" => CreateFrameworkElement<MasterCeeWindow>(),
            "Lista" => CreateFrameworkElement<ListaWindow>(),
            "Settings" => CreateFrameworkElement<SettingWindow>(),
            "Admin" => CreateFrameworkElement<AdminWindow>(),
            "Change Password" => CreateFrameworkElement<ChangePasswordWindow>(),
            "Help" => CreateFrameworkElement<HelpWindow>(),
            "Login" => CreateFrameworkElement<LoginWindow>(),
            "Logout" => CreateFrameworkElement<LogoutWindow>(),
            "Membretes" => CreateFrameworkElement<MembretesWindow>(),
            _ => throw new ArgumentException($"View {viewName} not found")
        };
    }

    private FrameworkElement CreateFrameworkElement<T>() where T : FrameworkElement
    {
        var view = _serviceProvider.GetRequiredService<T>();
        return view;
    }
}