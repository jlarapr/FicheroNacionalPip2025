using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class MasterCeeViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public MasterCeeViewModel() {
        MyTitle = "Master Cee";
    }
}