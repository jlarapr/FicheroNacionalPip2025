using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class MasterAfiliadosWindow  {
    public MasterAfiliadosWindow(MasterAfiliadosViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}