using System.Collections.ObjectModel;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Common;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class ChangePasswordViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;
    [ObservableProperty] private PackIconKind _passwordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private PackIconKind _confirmPasswordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private PackIconKind _currentPasswordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private bool _isPasswordVisible;
    [ObservableProperty] private bool _isConfirmPasswordVisible;
    [ObservableProperty] private bool _isCurrentPasswordVisible;
    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _resetPasswordBox = false;
    [ObservableProperty] private bool _resetConfirmPasswordBox = false;
    [ObservableProperty] private bool _resetCurrentPasswordBox = false;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isValidating = false;
    [ObservableProperty] private ObservableCollection<string> _validationErrors = new();
    [ObservableProperty] private string _currentUsername = string.Empty;

    private readonly ILogger<ChangePasswordViewModel> _logger;
    private readonly IPasswordPolicyService _passwordPolicyService;
    private readonly IUserManagementService _userManagementService;

    public IAsyncRelayCommand SaveCommand { get; }

    public ChangePasswordViewModel(
        ILogger<ChangePasswordViewModel> logger,
        IPasswordPolicyService passwordPolicyService,
        IUserManagementService userManagementService) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _passwordPolicyService = passwordPolicyService ?? throw new ArgumentNullException(nameof(passwordPolicyService));
        _userManagementService = userManagementService ?? throw new ArgumentNullException(nameof(userManagementService));

        MyTitle = "Change Password";
        PasswordIcon = PackIconKind.EyeOffOutline;
        ConfirmPasswordIcon = PackIconKind.EyeOffOutline;
        CurrentPasswordIcon = PackIconKind.EyeOffOutline;

        SaveCommand = new AsyncRelayCommand<object>(SaveAsyncWithParameter, CanSaveAsync);

        _logger.LogInformation("ChangePasswordViewModel inicializado");
    }

    private bool CanSaveAsync(object? parameter = null) {
        try {
            // Las contraseñas no deben estar vacías
            if (string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword) ||
                string.IsNullOrWhiteSpace(CurrentPassword)) {
                return false;
            }

            // Las contraseñas deben coincidir
            if (Password != ConfirmPassword) {
                return false;
            }

            // No permitir guardar mientras estamos validando
            return !IsValidating;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error inesperado al validar contraseñas");
            ErrorMessage = ex.Message;
            return false;
        }
    }

    private async Task SaveAsyncWithParameter(object? parameter) {
        try {
            IsValidating = true;
            ErrorMessage = string.Empty;
            ValidationErrors.Clear();

            if (string.IsNullOrEmpty(CurrentUsername)) {
                ErrorMessage = "ERROR: No se ha establecido el usuario actual. Por favor, inicie sesión.";
                ValidationErrors.Add("Debe iniciar sesión para cambiar su contraseña.");
                _logger.LogWarning("Intento de cambiar contraseña sin nombre de usuario establecido");
                IsValidating = false;
                return;
            }
            
            // Llamar directamente al servicio que ya implementa todas las validaciones
            Result<bool, string> changeResult = await _userManagementService.ChangePasswordAsync(
                CurrentUsername, CurrentPassword, Password);
                
            if (changeResult.IsFailure) {
                ErrorMessage = "ERROR: " + changeResult.GetErrorOrDefault();
                ValidationErrors.Add(changeResult.GetErrorOrDefault() ?? "Error desconocido");
                IsValidating = false;
                return;
            }
            
            // Si llegamos aquí, el cambio fue exitoso
            ErrorMessage = "ÉXITO: Contraseña cambiada correctamente";
            
            // Limpiar usando el control pasado como parámetro
            if (parameter is System.Windows.FrameworkElement control) {
                LimpiarFormulario(control);
                _logger.LogInformation("Formulario limpiado correctamente después de cambio exitoso");
            } else {
                // Si no hay parámetro, usar la limpieza básica
                LimpiarCampos();
                _logger.LogWarning("No se recibió control para limpiar los PasswordBox directamente");
            }
        }
        catch (Exception ex) {
            // Handle any exceptions and display the error message
            ErrorMessage = $"ERROR: Al cambiar la contraseña: {ex.Message}";
            _logger.LogError(ex, "Error al cambiar la contraseña");
        }
        finally {
            IsValidating = false;
        }
    }

    /// <summary>
    /// Limpia todos los campos del formulario de cambio de contraseña.
    /// </summary>
    private void LimpiarCampos() {
        try {
            _logger.LogDebug("Limpiando campos del formulario de cambio de contraseña");
            
            // Limpiar campos de texto
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            CurrentPassword = string.Empty;
            
            // Resetear estados de visibilidad
            IsPasswordVisible = false;
            IsConfirmPasswordVisible = false;
            IsCurrentPasswordVisible = false;
            
            // Resetear iconos
            PasswordIcon = PackIconKind.EyeOffOutline;
            ConfirmPasswordIcon = PackIconKind.EyeOffOutline;
            CurrentPasswordIcon = PackIconKind.EyeOffOutline;
            
            // Activar reseteo de PasswordBox
            ResetPasswordBox = true;
            ResetConfirmPasswordBox = true;
            ResetCurrentPasswordBox = true;
            
            // Limpiar errores de validación pero preservar el mensaje principal
            // ErrorMessage = string.Empty; // Comentado para preservar mensaje de éxito/error
            ValidationErrors.Clear();
            
            // Asegurar que se re-evalúe el estado del botón
            SaveCommand.NotifyCanExecuteChanged();
            
            _logger.LogDebug("Campos del formulario limpiados exitosamente");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al limpiar los campos del formulario");
            ErrorMessage = "Error al limpiar el formulario";
        }
    }

    /// <summary>
    /// Inicializa el ViewModel con el nombre de usuario
    /// </summary>
    public void Initialize(string username) {
        if (string.IsNullOrEmpty(username)) {
            _logger.LogWarning("Se intentó inicializar ChangePasswordViewModel con un nombre de usuario vacío");
            ErrorMessage = "No se ha establecido el usuario actual. Por favor, inicie sesión.";
            return;
        }

        CurrentUsername = username;
        _logger.LogInformation("ChangePasswordViewModel inicializado para el usuario: {Username}", username);
        //  ErrorMessage = string.Empty;
    }

    /// <summary>
    /// Comando para limpiar los campos del formulario
    /// </summary>
    [RelayCommand]
    private void LimpiarFormulario(object? parameter = null) {
        try {
            // Primero, limpiar las propiedades del ViewModel
            LimpiarCampos();
            
            // Si se está pasando un control como parámetro, limpiar directamente los campos
            if (parameter is System.Windows.FrameworkElement container) {
                _logger.LogDebug("Limpiando campos directamente de la interfaz de usuario");
                
                // Método 1: Buscar por nombre específico
                var passwordBox = FindPasswordBoxByName(container, "PasswordBox");
                var confirmPasswordBox = FindPasswordBoxByName(container, "PasswordBoxVerificar");
                var currentPasswordBox = FindPasswordBoxByName(container, "CurrentPasswordBox");
                
                // Limpiar los encontrados directamente
                passwordBox?.Clear();
                confirmPasswordBox?.Clear();
                currentPasswordBox?.Clear();
                
                // Método 2: Buscar todos los PasswordBox en el árbol visual
                ResetAllPasswordBoxes(parameter);
                
                // Forzar actualización de UI si es necesario
                if (System.Windows.Application.Current.Dispatcher.CheckAccess()) {
                    System.Windows.Application.Current.Dispatcher.Invoke(() => {
                        // Forzar actualización de UI
                        _logger.LogDebug("Forzando actualización de la interfaz de usuario");
                    });
                }
            }
            
            // Notificar que el comando puede haber cambiado su estado
            SaveCommand.NotifyCanExecuteChanged();
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al limpiar formulario");
            ErrorMessage = "Error al limpiar el formulario";
        }
    }

    /// <summary>
    /// Busca un control PasswordBox por nombre dentro de un contenedor
    /// </summary>
    private PasswordBox? FindPasswordBoxByName(System.Windows.FrameworkElement container, string name) {
        // Encontrar el PasswordBox por nombre
        if (container.FindName(name) is PasswordBox passwordBox) {
            return passwordBox;
        }

        return null;
    }

    [RelayCommand]
    private void TogglePasswordVisibility(object? parameter) {
        IsPasswordVisible = !IsPasswordVisible;
        PasswordIcon = IsPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is PasswordBox passwordBox && passwordBox.Name == "PasswordBox") {
            if (IsPasswordVisible) {
                // Guardamos el valor antes de cambiar la visibilidad
                Password = passwordBox.Password;
            }
            else {
                // Restauramos el valor después de cambiar la visibilidad
                passwordBox.Password = Password;
            }

            // Notificar a SaveCommand para re-evaluar si se debe habilitar
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility(object? parameter) {
        IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        ConfirmPasswordIcon = IsConfirmPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is PasswordBox passwordBox && passwordBox.Name == "PasswordBoxVerificar") {
            if (IsConfirmPasswordVisible) {
                // Guardamos el valor antes de cambiar la visibilidad
                ConfirmPassword = passwordBox.Password;
            }
            else {
                // Restauramos el valor después de cambiar la visibilidad
                passwordBox.Password = ConfirmPassword;
            }

            // Notificar a SaveCommand para re-evaluar si se debe habilitar
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    [RelayCommand]
    private void ToggleCurrentPasswordVisibility(object? parameter) {
        IsCurrentPasswordVisible = !IsCurrentPasswordVisible;
        CurrentPasswordIcon = IsCurrentPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is PasswordBox passwordBox && passwordBox.Name == "CurrentPasswordBox") {
            if (IsCurrentPasswordVisible) {
                // Guardamos el valor antes de cambiar la visibilidad
                CurrentPassword = passwordBox.Password;
            }
            else {
                // Restauramos el valor después de cambiar la visibilidad
                passwordBox.Password = CurrentPassword;
            }

            // Notificar a SaveCommand para re-evaluar si se debe habilitar
            SaveCommand.NotifyCanExecuteChanged();
        }
    }

    // Agregamos una propiedad observable para el cambio de Password y ConfirmPassword
    partial void OnPasswordChanged(string value) {
        // Notificar a SaveCommand que debe re-evaluar CanExecute
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmPasswordChanged(string value) {
        // Notificar a SaveCommand que debe re-evaluar CanExecute
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnCurrentPasswordChanged(string value) {
        // Notificar a SaveCommand que debe re-evaluar CanExecute
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void PasswordChanged(object? parameter) {
        if (parameter is PasswordBox passwordBox) {
            Password = passwordBox.Password;

            // Si se ha solicitado un reset del PasswordBox, limpiarlo ahora
            if (ResetPasswordBox) {
                passwordBox.Clear();
                ResetPasswordBox = false;
            }
        }
    }

    [RelayCommand]
    private void ConfirmPasswordChanged(object? parameter) {
        if (parameter is PasswordBox passwordBox) {
            ConfirmPassword = passwordBox.Password;

            // Si se ha solicitado un reset del PasswordBox, limpiarlo ahora
            if (ResetConfirmPasswordBox) {
                passwordBox.Clear();
                ResetConfirmPasswordBox = false;
            }
        }
    }

    [RelayCommand]
    private void CurrentPasswordChanged(object? parameter) {
        if (parameter is PasswordBox passwordBox) {
            CurrentPassword = passwordBox.Password;

            // Si se ha solicitado un reset del PasswordBox, limpiarlo ahora
            if (ResetCurrentPasswordBox) {
                passwordBox.Clear();
                ResetCurrentPasswordBox = false;
            }
        }
    }

    /// <summary>
    /// Resetea manualmente los PasswordBox a través de su controlador
    /// </summary>
    public void ResetAllPasswordBoxes(object? container) {
        if (container is System.Windows.DependencyObject depObj) {
            try {
                // Buscar todos los PasswordBox dentro del objeto visual
                var passwordBoxes = FindVisualChildren<PasswordBox>(depObj);
                int count = 0;
                
                foreach (var passwordBox in passwordBoxes) {
                    // Limpiar y loggear para debug
                    passwordBox.Clear();
                    passwordBox.Password = string.Empty; // Doble limpieza por seguridad
                    count++;
                }
                
                _logger.LogDebug("Limpiados {Count} controles PasswordBox", count);
                
                // Si no se encontraron, intentar buscar más arriba en el árbol visual
                if (count == 0 && container is System.Windows.FrameworkElement frameworkElement) {
                    if (frameworkElement.Parent is System.Windows.DependencyObject parent) {
                        _logger.LogDebug("Buscando PasswordBox en el nivel superior");
                        ResetAllPasswordBoxes(parent);
                    }
                }
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al intentar resetear PasswordBox");
            }
        }
    }

    /// <summary>
    /// Encuentra todos los controles de un tipo específico dentro de un elemento visual
    /// </summary>
    private static IEnumerable<T> FindVisualChildren<T>(System.Windows.DependencyObject depObj) where T : System.Windows.DependencyObject {
        if (depObj == null) yield break;

        for (int i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++) {
            var child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);

            if (child != null && child is T t) {
                yield return t;
            }

            foreach (var childOfChild in FindVisualChildren<T>(child)) {
                yield return childOfChild;
            }
        }
    }

    partial void OnCurrentUsernameChanged(string value) {
        // Si el username está vacío, mostrar un mensaje de error
        if (string.IsNullOrEmpty(value)) {
            ErrorMessage = "AVISO: No se ha establecido el usuario actual. Por favor, inicie sesión.";
            _logger.LogWarning("Se cambió CurrentUsername a un valor vacío");
        }
        else {
            _logger.LogInformation("Username actualizado: {Username}", value);
        }
    }

    partial void OnErrorMessageChanged(string value) {
        // Asegurar que nunca sea null
        if (value == null) {
            ErrorMessage = string.Empty;
        }

        // Si hay un mensaje de error, logearlo para debug
        if (!string.IsNullOrEmpty(value)) {
            _logger.LogDebug("Mensaje de error establecido: {ErrorMessage}", value);
        }
    }
}