using System.Windows;
using FicheroNacionalPip.Presentation.Interfaces;
using FicheroNacionalPip.Presentation.ViewModels.LeftMenu;
using FicheroNacionalPip.Presentation.Views.LeftMenu;

namespace FicheroNacionalPip.Presentation.Services;

public class DialogService : IDialogService
{
    public bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class
    {
        // Mapeo de ViewModels a Vistas
        Window? dialog = viewModel switch
        {
            ElectorDialogViewModel vm => new ElectorDialog { DataContext = vm },
            // Agregar más casos según sea necesario
            _ => throw new ArgumentException($"No se encontró una vista para el ViewModel de tipo {typeof(TViewModel).Name}")
        };

        return dialog.ShowDialog();
    }

    public void ShowInfo(string message, string title = "Información")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public void ShowError(string message, string title = "Error")
    {
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public bool ShowConfirmation(string message, string title = "Confirmación")
    {
        var result = MessageBox.Show(
            message,
            title,
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        return result == MessageBoxResult.Yes;
    }
} 