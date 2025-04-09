using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class LoginViewModel : ObservableObject {

    [ObservableProperty] private string _myTitle;

    public LoginViewModel() {
        MyTitle = "Login";
    }

}