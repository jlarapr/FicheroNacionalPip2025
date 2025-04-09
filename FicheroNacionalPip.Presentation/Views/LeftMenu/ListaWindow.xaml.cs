using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class ListaWindow  {
    public ListaWindow(ListaViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}