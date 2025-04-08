namespace FicheroNacionalPip.Presentation.Interfaces;

public interface IClosingWindows {
    Action? OnClosing { get; set; }
    bool CanClose();
}