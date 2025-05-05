using System.Windows;

namespace FicheroNacionalPip.Presentation.Interfaces;

public interface IDialogService
{
    /// <summary>
    /// Muestra un diálogo modal con el ViewModel especificado
    /// </summary>
    /// <typeparam name="TViewModel">Tipo del ViewModel</typeparam>
    /// <param name="viewModel">ViewModel que se usará como contexto de datos</param>
    /// <returns>Resultado del diálogo (true si se confirmó, false si se canceló, null si se cerró)</returns>
    bool? ShowDialog<TViewModel>(TViewModel viewModel) where TViewModel : class;

    /// <summary>
    /// Muestra un mensaje de información
    /// </summary>
    /// <param name="message">Mensaje a mostrar</param>
    /// <param name="title">Título de la ventana</param>
    void ShowInfo(string message, string title = "Información");

    /// <summary>
    /// Muestra un mensaje de error
    /// </summary>
    /// <param name="message">Mensaje de error</param>
    /// <param name="title">Título de la ventana</param>
    void ShowError(string message, string title = "Error");

    /// <summary>
    /// Muestra un mensaje de confirmación
    /// </summary>
    /// <param name="message">Mensaje de confirmación</param>
    /// <param name="title">Título de la ventana</param>
    /// <returns>True si se confirmó, False si se canceló</returns>
    bool ShowConfirmation(string message, string title = "Confirmación");
} 