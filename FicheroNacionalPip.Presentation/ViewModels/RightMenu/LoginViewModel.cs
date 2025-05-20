using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Presentation.Interfaces;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class LoginViewModel : ObservableObject {
    [ObservableProperty] private PackIconKind _passwordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private string _myTitle;
    [ObservableProperty] private string _username = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isPasswordVisible;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isLoggingIn;
    [ObservableProperty] private bool _resetPasswordBox = false;

    private readonly IAuthenticationService _authService;
    private readonly ILogger<LoginViewModel> _logger;
    private readonly IViewService _viewService;

    // Evento para notificar al MainViewModel sobre un inicio de sesión exitoso
    public event Action<UserAuthInfo>? LoginSuccessful;

    public LoginViewModel(
        IAuthenticationService authService,
        ILogger<LoginViewModel> logger,
        IViewService viewService) {
        _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));

        MyTitle = "Login";
        PasswordIcon = PackIconKind.EyeOffOutline;

        _logger.LogDebug("LoginViewModel inicializado");
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
    private void TogglePasswordVisibility(object? parameter) {
        IsPasswordVisible = !IsPasswordVisible;
        PasswordIcon = IsPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is PasswordBox passwordBox) {
            if (IsPasswordVisible) {
                // Guardamos el valor antes de cambiar la visibilidad
                Password = passwordBox.Password;
            }
            else {
                // Restauramos el valor después de cambiar la visibilidad
                passwordBox.Password = Password;
            }
        }
    }
    
    /// <summary>
    /// Restablece el estado de la vista de login, limpiando todos los campos incluyendo el PasswordBox.
    /// </summary>
    public void Reset() {
        _logger.LogDebug("Restableciendo vista de login");
        Username = string.Empty;
        Password = string.Empty;
        ErrorMessage = string.Empty;
        IsPasswordVisible = false;
        PasswordIcon = PackIconKind.EyeOffOutline;
        
        // Activar la señal para limpiar el PasswordBox
        ResetPasswordBox = true;
        
        _logger.LogDebug("Vista de login restablecida");
    }

    [RelayCommand]
    private void Login() {
        ErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(Username)) {
            ErrorMessage = "El nombre de usuario es requerido";
            return;
        }

        if (string.IsNullOrWhiteSpace(Password)) {
            ErrorMessage = "La contraseña es requerida";
            return;
        }

        try {
            IsLoggingIn = true;
            _logger.LogInformation("Iniciando proceso de login para usuario: {Username}", Username);

            Result<UserAuthInfo, string> result = _authService.Authenticate(Username, Password);

            if (result.IsSuccess) {
                // Obtenemos el usuario y verificamos que no sea nulo
                UserAuthInfo? userInfo = result.GetValueOrDefault();
                if (userInfo == null) {
                    ErrorMessage = "Error de autenticación: información de usuario no disponible";
                    _logger.LogWarning("Error de autenticación: información de usuario nula");
                    return;
                }

                _logger.LogInformation("Usuario autenticado correctamente: {Username}", Username);

                // Limpiar campos de login
                Username = string.Empty;
                Password = string.Empty;
                
                // Notificar al MainViewModel sobre el inicio de sesión exitoso
                LoginSuccessful?.Invoke(userInfo);
            }
            else {
                ErrorMessage = result.GetErrorOrDefault() ?? "Error de autenticación desconocido";
                _logger.LogWarning("Error de autenticación: {ErrorMessage}", ErrorMessage);
            }
        }
        catch (Exception ex) {
            ErrorMessage = $"Error al iniciar sesión: {ex.Message}";
            _logger.LogError(ex, "Excepción durante el proceso de login");
        }
        finally {
            IsLoggingIn = false;
        }
    }
}