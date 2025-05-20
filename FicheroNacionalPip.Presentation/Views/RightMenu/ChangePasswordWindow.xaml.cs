using FicheroNacionalPip.Presentation.ViewModels.RightMenu;
using System.Windows.Controls;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class ChangePasswordWindow : UserControl {
    public ChangePasswordWindow(ChangePasswordViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
        
        // Obtener el username del ViewModel principal si está disponible
        var mainViewModel = App.Current.MainWindow?.DataContext as FicheroNacionalPip.Presentation.ViewModels.MainBaseMainWindowViewModel;
        if (mainViewModel != null && !string.IsNullOrEmpty(mainViewModel.UserName))
        {
            viewModel.Initialize(mainViewModel.UserName);
        }
    }
}