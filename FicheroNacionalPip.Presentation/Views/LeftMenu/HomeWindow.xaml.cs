using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class HomeWindow : UserControl
{
    public HomeWindow(HomeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}