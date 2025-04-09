using HomeViewModel = FicheroNacionalPip.Presentation.ViewModels.LeftMenu.HomeViewModel;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class HomeWindow  {
    public HomeWindow(HomeViewModel viewModel ) {
        InitializeComponent();
        DataContext = viewModel;
    }
}