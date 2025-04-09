using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class ChangePasswordViewModel: ObservableObject {
    [ObservableProperty] private string _myTitle;

    public ChangePasswordViewModel() {
        MyTitle = "Change Password";
    }
}