using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class MembretesWindow  {
    public MembretesWindow(MembretesViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}