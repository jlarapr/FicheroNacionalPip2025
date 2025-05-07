using System.Collections.ObjectModel;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace FicheroNacionalPip.Presentation.Models;

/// <summary>
/// Representa un ítem de menú en la interfaz de usuario.
/// </summary>
public record class MenuItemModel
{
    /// <summary>
    /// Nombre a mostrar en el menú.
    /// </summary>
    public string? DisplayName { get; init; }

    /// <summary>
    /// Ícono a mostrar junto al nombre.
    /// </summary>
    public PackIconKind? IconKind { get; init; }

    /// <summary>
    /// Comando a ejecutar cuando se selecciona el ítem.
    /// </summary>
    public ICommand? Command { get; init; }

    /// <summary>
    /// Colección de subítems del menú.
    /// </summary>
    public ObservableCollection<MenuItemModel>? SubItems { get; init; } = [];
}

