using System.Windows;

namespace FicheroNacionalPip.Presentation.Interfaces;

public interface IToggleMenu {
    public Action<bool>? ToggleMenuEvent { get; set; }
}