using AdminViewModel = FicheroNacionalPip.Presentation.ViewModels.RightMenu.AdminViewModel;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class AdminWindow  {
    public AdminWindow(AdminViewModel adminViewModel) {
        InitializeComponent();
        DataContext = adminViewModel;
    }

    private void CurrentPasswordTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
    {

    }
}