using System.Collections.ObjectModel;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;

namespace FicheroNacionalPip.Presentation.Models;

public class MenuItemModel {
    public string? DisplayName { get; set; }
    public PackIconKind? IconKind { get; set; }
    public ICommand? Command { get; set; }
    public ObservableCollection<MenuItemModel>? SubItems { get; set; } = [];
}