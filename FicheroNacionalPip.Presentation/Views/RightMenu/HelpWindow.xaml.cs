using FicheroNacionalPip.Presentation.ViewModels.RightMenu;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class HelpWindow  {
    public HelpWindow(HelpViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}