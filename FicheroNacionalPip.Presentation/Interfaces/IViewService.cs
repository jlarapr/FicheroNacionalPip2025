using System.Windows;

namespace FicheroNacionalPip.Presentation.Interfaces;

public interface IViewService {
    FrameworkElement GetView(string viewName);

}