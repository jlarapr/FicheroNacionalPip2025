using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels;
using SettingViewModel = FicheroNacionalPip.Presentation.ViewModels.RightMenu.SettingViewModel;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class SettingWindow : UserControl {
    public SettingWindow(SettingViewModel viewModel) {
        InitializeComponent();
        DataContext = viewModel;
    }
}