using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace FicheroNacionalPip.Presentation.Models;

/// <summary>
/// Representa un ítem de menú en la interfaz de usuario.
/// </summary>
public class MenuItemModel : INotifyPropertyChanged
{
    private bool _isEnabled = true;
    
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

    /// <summary>
    /// Indica si el ítem de menú está habilitado o no.
    /// </summary>
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled != value)
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

