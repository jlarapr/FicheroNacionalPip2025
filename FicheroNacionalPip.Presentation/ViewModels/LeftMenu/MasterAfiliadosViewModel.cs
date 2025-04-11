using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class MasterAfiliadosViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public MasterAfiliadosViewModel() {
        MyTitle = "Master Afiliados.";
    }
}