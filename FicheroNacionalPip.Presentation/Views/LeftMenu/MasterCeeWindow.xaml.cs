using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class MasterCeeWindow  {
    public MasterCeeWindow(MasterCeeViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}