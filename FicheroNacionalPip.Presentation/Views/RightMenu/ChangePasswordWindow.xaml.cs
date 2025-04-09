using FicheroNacionalPip.Presentation.ViewModels.RightMenu;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class ChangePasswordWindow  {
    public ChangePasswordWindow(ChangePasswordViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}