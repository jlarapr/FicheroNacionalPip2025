using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class MasterAfiliadosWindow : UserControl
{
    public MasterAfiliadosWindow(MasterAfiliadosViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}