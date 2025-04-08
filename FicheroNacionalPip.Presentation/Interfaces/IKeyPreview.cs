using System.Windows.Input;

namespace FicheroNacionalPip.Presentation.Interfaces;

public interface IKeyPreview {
    void KeyPreviewKeyDown(object sender, KeyEventArgs e);
}