using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class AdminViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public AdminViewModel() {
        MyTitle = "Admin";
    }
}