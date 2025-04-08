using System.Windows;
using FicheroNacionalPip.Presentation.ViewModels;

namespace FicheroNacionalPip.Presentation.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
    public MainWindow(MainBaseMainWindowViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}