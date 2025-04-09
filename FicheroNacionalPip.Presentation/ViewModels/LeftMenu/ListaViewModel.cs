using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class ListaViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    public ListaViewModel() {
        MyTitle = "Lista";
    }
}