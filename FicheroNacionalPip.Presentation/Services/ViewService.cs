using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Views;
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
            "Home" => _serviceProvider.GetRequiredService<HomeWindow>(),
            // Agrega otros casos según las vistas que tengas
            _ => throw new ArgumentException("Vista desconocida", nameof(viewName))
        };
    }

}