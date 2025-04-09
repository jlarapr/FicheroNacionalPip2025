using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class HelpViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public HelpViewModel() {
        MyTitle = "Help";
    }
}