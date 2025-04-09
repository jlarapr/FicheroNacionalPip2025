using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class MembretesViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public MembretesViewModel() {
        MyTitle = "Membretes";
    }
}