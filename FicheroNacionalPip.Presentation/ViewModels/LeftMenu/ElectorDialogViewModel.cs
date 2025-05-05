using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Presentation.Models;
using FicheroNacionalPip.Presentation.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Printing;

namespace FicheroNacionalPip.Presentation.ViewModels.LeftMenu;

public partial class ElectorDialogViewModel : ObservableObject, IClosingWindows {
    [ObservableProperty] private Elector _elector;
    [ObservableProperty] private bool? _dialogResult;

    public Action? OnClosing { get; set; }

    public ElectorDialogViewModel(Elector elector) {
        _elector = elector;
    }

    // Constructor para el diseador (Design Time)
    public ElectorDialogViewModel() : this(new Elector
    {
        Nombre = "Juan",
        ApellidoPaterno = "Pérez",
        ApellidoMaterno = "López",
        EstatusElectoral = "Activo",
        Sexo = "M",
        DireccionPostal1 = "Calle Falsa 123"
    })
    { }

    [RelayCommand]
    private void Save()
    {
        DialogResult = true;
        OnClosing?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        DialogResult = false;
        OnClosing?.Invoke();
    }

    [RelayCommand]
    private void Print()
    {
        try
        {
            PrintDialog printDialog = new();
            if (printDialog.ShowDialog() == true)
            {
                MessageBox.Show("Imprimiendo datos del elector...", "Impresión", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al imprimir: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public bool CanClose() => true;
}