using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class SettingViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public SettingViewModel() {
        MyTitle = "Settings";
    }
}