using FicheroNacionalPip.Presentation.ViewModels.RightMenu;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class LogoutWindow {
    public LogoutWindow(LogoutViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}