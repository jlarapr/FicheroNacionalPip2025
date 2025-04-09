using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Views;
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
        // Puedes utilizar un switch, un diccionario o cualquier
        // mecanismo para mapear el identificador con la vista concreta.
        return viewName switch
        {
             "Home"           => _serviceProvider.GetRequiredService<HomeWindow>(),
             "Settings"       => _serviceProvider.GetRequiredService<SettingWindow>(),
             "Admin"          => _serviceProvider.GetRequiredService<AdminWindow>(),
             "Change Password" => _serviceProvider.GetRequiredService<ChangePasswordWindow>(),
             "Help"           => _serviceProvider.GetRequiredService<HelpWindow>(),
             "Login"          => _serviceProvider.GetRequiredService<LoginWindow>(),
             "Logout"         => _serviceProvider.GetRequiredService<LogoutWindow>(),
             "Master Afiliados" => _serviceProvider.GetRequiredService<MasterAfiliadosWindow>(),
             "Master CEE" => _serviceProvider.GetRequiredService<MasterCeeWindow>(),
             "Lista"          => _serviceProvider.GetRequiredService<ListaWindow>(),
             "Membretes" => _serviceProvider.GetRequiredService<MembretesWindow>(),

            // Agrega otros casos según las vistas que tengas
            _ => _serviceProvider.GetRequiredService<HomeWindow>()
        };
    }

}