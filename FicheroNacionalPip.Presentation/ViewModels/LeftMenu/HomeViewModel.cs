using CommunityToolkit.Mvvm.ComponentModel;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class HomeViewModel : ObservableObject {
    [ObservableProperty] private string _version;
    [ObservableProperty] private string _welcomeMessage;

    public HomeViewModel() {
        WelcomeMessage = string.Empty;
        var version = new FicheroNacionalPip.Business.Version();
        Version = version.ToString();
    }
}