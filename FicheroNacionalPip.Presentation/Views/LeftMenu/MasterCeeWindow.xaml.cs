using System.Windows.Controls;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

namespace FicheroNacionalPip.Presentation.Views.LeftMenu;

public partial class MasterCeeWindow : UserControl
{
    public MasterCeeWindow(MasterCeeViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}