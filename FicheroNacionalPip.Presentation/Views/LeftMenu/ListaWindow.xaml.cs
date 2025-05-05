using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class ListaWindow : UserControl
{
    public ListaWindow(ListaViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}