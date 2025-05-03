using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using FicheroNacionalPip.Presentation.Models;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class MasterCeeViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;
    [ObservableProperty] private ObservableCollection<Elector> _electors;

    public MasterCeeViewModel() {
        MyTitle = "Investigación demográfica";
        Electors = new ObservableCollection<Elector>
        {
            new Elector { Nombre = "Juan", Apellido = "Pérez" },
            new Elector { Nombre = "María", Apellido = "Gómez" }
        };

        
    }
}