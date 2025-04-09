using FicheroNacionalPip.Presentation.ViewModels.RightMenu;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class LoginWindow  {
    public LoginWindow(LoginViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}