using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels;

namespace FicheroNacionalPip.Presentation.Views;

public partial class HomeWindow : UserControl {
    public HomeWindow(HomeViewModel viewModel ) {
        InitializeComponent();
        DataContext = viewModel;
    }
}