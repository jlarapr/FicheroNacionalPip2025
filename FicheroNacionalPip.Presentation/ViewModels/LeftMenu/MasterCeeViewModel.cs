using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.Models;
using FicheroNacionalPip.Presentation.Views.LeftMenu;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class MasterCeeViewModel : ObservableObject {
    private readonly IDialogService _dialogService;

    public ObservableCollection<Elector> MyElectores { get; } = new() {
        new Elector {
            NumeroElectoral = "1001",
            Nombre = "Juan",
            ApellidoPaterno = "Ramírez",
            ApellidoMaterno = "Torres",
            EstatusElectoral = "Activo",
            FechaNacimiento = new System.DateTime(1990, 1, 15),
            NombrePadre = "Carlos Ramírez",
            NombreMadre = "Elena Torres",
            Sexo = "Masculino",
            Gemelo = false,
            FuncionarioDeColegio = true,
            DireccionPostal1 = "Av. Principal 123"
        },
        new Elector {
            NumeroElectoral = "1002",
            Nombre = "María",
            ApellidoPaterno = "López",
            ApellidoMaterno = "Gómez",
            EstatusElectoral = "Inactivo",
            FechaNacimiento = new System.DateTime(1985, 3, 20),
            NombrePadre = "Luis López",
            NombreMadre = "Clara Gómez",
            Sexo = "Femenino",
            Gemelo = true,
            FuncionarioDeColegio = false,
            DireccionPostal1 = "Calle 5, Edif. 8"
        }
    };
    private bool CanOpenDialog() => SelectedElector != null;

    [ObservableProperty] private Elector? _selectedElector;
    [ObservableProperty] private string _myTitle;
    [ObservableProperty] private ObservableCollection<Elector> _electors;

    // Constructor for runtime
    public MasterCeeViewModel(IDialogService dialogService) {
        _dialogService = dialogService;
        MyTitle = "Investigación demográfica";
        Electors = MyElectores;
    }

    // Constructor for design-time
    public MasterCeeViewModel() : this(new DesignTimeDialogService()) { }

    [RelayCommand(CanExecute = nameof(CanOpenDialog))]
    private void OpenDialog() {
        if (SelectedElector == null)
            return;

        var dialogViewModel = new ElectorDialogViewModel(SelectedElector);
        var result = _dialogService.ShowDialog(dialogViewModel);

        if (result == true) {
            // Los cambios ya están aplicados a la instancia directamente
            _dialogService.ShowInfo("Los cambios se han guardado correctamente.", "Éxito");
        }
    }
}

// Design-time mock service
internal class DesignTimeDialogService : IDialogService
{
    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class => null;
    public void ShowInfo(string message, string title = "Información") { }
    public void ShowError(string message, string title = "Error") { }
    public bool ShowConfirmation(string message, string title = "Confirmación") => false;
}