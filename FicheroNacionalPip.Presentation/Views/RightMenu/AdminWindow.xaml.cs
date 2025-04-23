using AdminViewModel = FicheroNacionalPip.Presentation.ViewModels.RightMenu.AdminViewModel;

namespace FicheroNacionalPip.Presentation.Views.RightMenu;

public partial class AdminWindow  {
    public AdminWindow(AdminViewModel adminViewModel) {
        InitializeComponent();
        DataContext = adminViewModel;
    }

 
}