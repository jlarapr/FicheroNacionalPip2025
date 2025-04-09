using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class LogoutViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public LogoutViewModel() {
        MyTitle = "Logout";
    }
}