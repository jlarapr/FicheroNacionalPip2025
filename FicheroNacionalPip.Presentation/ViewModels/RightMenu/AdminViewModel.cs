using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Business.Services;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using FicheroNacionalPip.Common;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class AdminViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;
    [ObservableProperty] private PackIconKind _passwordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private PackIconKind _confirmPasswordIcon = PackIconKind.EyeOff;
    [ObservableProperty] private bool _isConfirmPasswordVisible;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private bool _resetPasswordBox;
    [ObservableProperty] private bool _resetConfirmPasswordBox;
    [ObservableProperty] private bool _isPasswordVisible;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private bool _isValidating;
    [ObservableProperty] private ObservableCollection<string> _validationErrors = new();
    [ObservableProperty] private ObservableCollection<UserDto> _users = new();
    [ObservableProperty] private UserDto? _selectedUser;
    [ObservableProperty] private string _newUserName = string.Empty;
    [ObservableProperty] private bool _isComboBoxEditable;
    [ObservableProperty] private bool _areButtonsEnabled = true;

    public IAsyncRelayCommand SaveCommand { get; }
    public IAsyncRelayCommand AddUserCommand { get; }
    public IAsyncRelayCommand EditUserCommand { get; }
    public IAsyncRelayCommand DeleteUserCommand { get; }
    public IAsyncRelayCommand LoadUsersCommand { get; }

    private readonly ILogger<AdminViewModel> _logger;
    private readonly IUserAdministrationService _userAdministrationService;

    public AdminViewModel(ILogger<AdminViewModel> logger, IUserAdministrationService userAdministrationService) {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _userAdministrationService = userAdministrationService ?? throw new ArgumentNullException(nameof(userAdministrationService));

        MyTitle = "User maintenance";
        PasswordIcon = PackIconKind.EyeOffOutline;
        ConfirmPasswordIcon = PackIconKind.EyeOffOutline;
        
        SaveCommand = new AsyncRelayCommand<object>(SaveAsyncWithParameter, CanSaveAsync);
        AddUserCommand = new AsyncRelayCommand<object>(AddUserAsync, CanAddUserAsync);
        EditUserCommand = new AsyncRelayCommand<object>(EditUserAsync, CanEditUserAsync);
        DeleteUserCommand = new AsyncRelayCommand<object>(DeleteUserAsync, CanDeleteUserAsync);
        LoadUsersCommand = new AsyncRelayCommand(LoadUsersAsync);

        ErrorMessage = "AdminViewModel inicializado";
        _logger.LogInformation("AdminViewModel inicializado");

        // Load users when the ViewModel is initialized
        LoadUsersCommand.Execute(null);
        IsComboBoxEditable =  true; // TODO: esto es teporero
    }

    private bool CanDeleteUserAsync(object? obj) {
        return !IsValidating && obj != null;
    }

    private async Task DeleteUserAsync(object? arg) {
        if (arg is not string userId || string.IsNullOrWhiteSpace(userId)) {
            _logger.LogWarning("DeleteUserAsync was called with an invalid userId.");
            ErrorMessage = "Invalid user ID.";
            return;
        }

        try {
            IsValidating = true;
            _logger.LogInformation("Attempting to delete user with ID: {UserId}", userId);
            
            Result<bool, string> deleteResult = await _userAdministrationService.DeleteUserAsync(userId);
            
            if (deleteResult.IsFailure)
            {
                ErrorMessage = $"Error: deleting user: {deleteResult.GetErrorOrDefault()}";
                ValidationErrors.Add(deleteResult.GetErrorOrDefault() ?? ErrorMessage);
                IsValidating = false;
                return;
            }

            _logger.LogInformation("User with ID: {UserId} successfully deleted.", userId);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while deleting user with ID: {UserId}", userId);
            ErrorMessage = "An error occurred while deleting the user. Please try again.";
        }
        finally {
            IsValidating = false;
        }
    }

    private bool CanEditUserAsync(object? obj) {
        if (obj is not UserAuthInfo user) {
            _logger.LogWarning("CanEditUserAsync: Invalid object type or null.");
            return false;
        }

        bool canEdit = !string.IsNullOrWhiteSpace(user.UserName); //TODO: verificar esto -> && user.IsActive;

        if (!canEdit) _logger.LogInformation("CanEditUserAsync: User cannot be edited due to missing username or inactive status.");

        return canEdit;
    }

    private async Task EditUserAsync(object? arg) {
        if (arg is not UserDto user) {
            _logger.LogWarning("EditUserAsync called with invalid argument.");
            ErrorMessage = "Invalid user data.";
            return;
        }

        try {
            IsValidating = true;
            ValidationErrors.Clear();

            // Perform validation
            if (string.IsNullOrWhiteSpace(user.UserName)) ValidationErrors.Add("User name cannot be empty.");

            if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@")) ValidationErrors.Add("Invalid email address.");

            if (ValidationErrors.Count > 0) {
                ErrorMessage = "Validation errors occurred.";
                return;
            }

            Result<bool, string> editResult = await _userAdministrationService.EditUserAsync(user);

            if (editResult.IsFailure) {
                ErrorMessage = $"Error: deleting user: {editResult.GetErrorOrDefault()}";
                ValidationErrors.Add(editResult.GetErrorOrDefault() ?? ErrorMessage);
                IsValidating = false;
                return;
            }

            _logger.LogInformation("User {UserUserName} edited successfully.", user.UserName);
            ErrorMessage = string.Empty;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "An error occurred while editing the user.");
            ErrorMessage = "An error occurred while editing the user.";
        }
        finally {
            IsValidating = false;
        }
    }

    private bool CanAddUserAsync(object? obj) {
        return AreButtonsEnabled;
    }

    private async Task AddUserAsync(object? arg) {
        try {
            IsComboBoxEditable = true;
            AreButtonsEnabled = false;
            ErrorMessage = string.Empty;
        }
        finally {
            IsValidating = false;
        }
    }

    private async Task SaveAsyncWithParameter(object? parameter) {
        try {
            IsValidating = true;
            ErrorMessage = string.Empty;
            ValidationErrors.Clear();
            
            // return method no implemented yet
            await Task.Delay(100);
            // Simulate saving data
            
            
            _logger.LogInformation("ÉXITO: Cambios guardados correctamente");
            ErrorMessage = "ÉXITO: Cambios guardados correctamente";
            // Limpiar usando el control pasado como parámetro
            if (parameter is FrameworkElement control)
            {
                LimpiarFormulario(control);
                _logger.LogInformation("Formulario limpiado correctamente después de cambio exitoso");
            }
            else
            {
                // Si no hay parámetro, usar la limpieza básica
                LimpiarCampos();
                _logger.LogWarning("No se recibió control para limpiar los PasswordBox directamente");
            }
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al guardar los cambios");
            ErrorMessage = "Error al guardar los cambios";
        }
        finally
        {
            IsValidating = false;
        }
    }

    private bool CanSaveAsync(object? parameter = null) {
        try {
            // Las contraseñas no deben estar vac�as
            if (string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword)) {
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

    [RelayCommand]
    private void ConfirmPasswordChanged(object? parameter) {
        if (parameter is not PasswordBox passwordBox) return;

        ConfirmPassword = passwordBox.Password;

        // Si se ha solicitado un reset del PasswordBox, limpiarlo ahora
        if (!ResetConfirmPasswordBox) return;

        passwordBox.Clear();
        ResetConfirmPasswordBox = false;
    }

    [RelayCommand]
    private void TogglePasswordVisibility(object? parameter) {
        IsPasswordVisible = !IsPasswordVisible;
        PasswordIcon = IsPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is not PasswordBox { Name: "PasswordBox" } passwordBox) return;

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

    [RelayCommand]
    private void PasswordChanged(object? parameter)
    {
        if (parameter is not PasswordBox passwordBox) return;
        
        Password = passwordBox.Password;

        // Si se ha solicitado un reset del PasswordBox, limpiarlo ahora
        if (!ResetPasswordBox) return;
        
        passwordBox.Clear();
        ResetPasswordBox = false;
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility(object? parameter) {
        IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        ConfirmPasswordIcon = IsConfirmPasswordVisible ? PackIconKind.EyeOutline : PackIconKind.EyeOffOutline;

        if (parameter is not PasswordBox { Name: "PasswordBoxVerificar" } passwordBox) return;

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

    /// <summary>
    /// Comando para limpiar los campos del formulario
    /// </summary>
    [RelayCommand]
    private void LimpiarFormulario(object? parameter = null) {
        try {
            // Primero, limpiar las propiedades del ViewModel
            LimpiarCampos();

            // Si se est� pasando un control como par�metro, limpiar directamente los campos
            if (parameter is FrameworkElement container) {
                _logger.LogDebug("Limpiando campos directamente de la interfaz de usuario");

                // Método 1: Buscar por nombre específico
                PasswordBox? passwordBox = FindPasswordBoxByName(container, "PasswordBox");
                PasswordBox? confirmPasswordBox = FindPasswordBoxByName(container, "PasswordBoxVerificar");
                
                // Limpiar los encontrados directamente
                passwordBox?.Clear();
                confirmPasswordBox?.Clear();
                
                // M�todo 2: Buscar todos los PasswordBox en el arbor visual
                ResetAllPasswordBoxes(parameter);

                // Forzar actualización de UI si es necesario
                if (Application.Current.Dispatcher.CheckAccess()) {
                    Application.Current.Dispatcher.Invoke(() => {
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
    /// Limpia todos los campos del formulario de cambio de contraseña.
    /// </summary>
    private void LimpiarCampos() {
        try {
            _logger.LogDebug("Limpiando campos del formulario de cambio de contraseña");

            // Limpiar campos de texto
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            ErrorMessage = string.Empty;
            Email = string.Empty;

            // Resetear estados de visibilidad
            IsPasswordVisible = false;
            IsConfirmPasswordVisible = false;

            // Resetear iconos
            PasswordIcon = PackIconKind.EyeOffOutline;
            ConfirmPasswordIcon = PackIconKind.EyeOffOutline;

            // Activar reseteo de PasswordBox
            ResetPasswordBox = true;
            ResetConfirmPasswordBox = true;

            // Limpiar errores de validación pero preservar el mensaje principal
            ValidationErrors.Clear();

            // Asegurar que se reevalúe el estado del botón.
            SaveCommand.NotifyCanExecuteChanged();

            _logger.LogDebug("Campos del formulario limpiados exitosamente");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al limpiar los campos del formulario");
            ErrorMessage = "Error al limpiar el formulario";
        }
    }

    /// <summary>
    /// Busca un control PasswordBox por nombre dentro de un contenedor
    /// </summary>
    private PasswordBox? FindPasswordBoxByName(FrameworkElement container, string name) {
        // Encontrar el PasswordBox por nombre
        if (container.FindName(name) is PasswordBox passwordBox) {
            return passwordBox;
        }

        return null;
    }

    /// <summary>
    /// Resetea manualmente los PasswordBox a trav�s de su controlador
    /// </summary>
    public void ResetAllPasswordBoxes(object? container) {
        if (container is DependencyObject depObj) {
            try {
                // Buscar todos los PasswordBox dentro del objeto visual
                IEnumerable<PasswordBox> passwordBoxes = FindVisualChildren<PasswordBox>(depObj);
                var count = 0;

                foreach (PasswordBox passwordBox in passwordBoxes) {
                    // Limpiar y loggear para debug
                    passwordBox.Clear();
                    passwordBox.Password = string.Empty; // Doble limpieza por seguridad
                    count++;
                }

                _logger.LogDebug("Limpiados {Count} controles PasswordBox", count);

                // Si no se encontraron, intentar buscar mas arriba en el arbor visual
                if (count != 0 || container is not FrameworkElement frameworkElement) return;

                if (frameworkElement.Parent is not { } parent) return;

                _logger.LogDebug("Buscando PasswordBox en el nivel superior");
                ResetAllPasswordBoxes(parent);
            }
            catch (Exception ex) {
                _logger.LogError(ex, "Error al intentar resetear PasswordBox");
            }
        }
    }

    /// <summary>
    /// Encuentra todos los controles de un tipo específico dentro de un elemento visual
    /// </summary>
    private static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject {
        if (depObj == null) yield break;

        for (var i = 0; i < System.Windows.Media.VisualTreeHelper.GetChildrenCount(depObj); i++) {
            DependencyObject? child = System.Windows.Media.VisualTreeHelper.GetChild(depObj, i);

            if (child is T t) {
                yield return t;
            }

            foreach (T childOfChild in FindVisualChildren<T>(child)) {
                yield return childOfChild;
            }
        }
    }

    // Agregamos una propiedad observable para el cambio de Password y ConfirmPassword
    partial void OnPasswordChanged(string value)
    {
        // Notificar a SaveCommand que debe re-evaluar CanExecute
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnConfirmPasswordChanged(string value)
    {
        // Notificar a SaveCommand que debe re-evaluar CanExecute
        SaveCommand.NotifyCanExecuteChanged();
    }

    partial void OnErrorMessageChanged(string value)
    {
        // Asegurar que nunca sea null
        if (value == null)
        {
            ErrorMessage = string.Empty;
        }

        // Si hay un mensaje de error, logearlo para debug
        if (!string.IsNullOrEmpty(value))
        {
            _logger.LogDebug("Mensaje de error establecido: {ErrorMessage}", value);
        }
    }

    private async Task LoadUsersAsync() {
        try {
            IsValidating = true;
            Users.Clear();

            List<UserDto> users = await _userAdministrationService.GetAllUsersAsync();
            foreach (UserDto user in users) {
                Users.Add(user);
            }

            _logger.LogInformation("Usuarios cargados exitosamente.");
        } catch (Exception ex) {
            _logger.LogError(ex, "Error al cargar usuarios.");
            ErrorMessage = "Error al cargar usuarios.";
        } finally {
            IsValidating = false;
        }
    }

    private async Task AddOrSelectUserAsync() {
        if (string.IsNullOrWhiteSpace(NewUserName)) {
            ErrorMessage = "Username cannot be empty.";
            return;
        }

        var existingUser = Users.FirstOrDefault(u => u.UserName.Equals(NewUserName, StringComparison.OrdinalIgnoreCase));
        if (existingUser != null) {
            SelectedUser = existingUser;
            return;
        }

        try {
            IsValidating = true;
            var newUser = new UserDto { UserName = NewUserName };
            var result = await _userAdministrationService.AddUserAsync(newUser);

            if (result.IsFailure) {
                ErrorMessage = result.GetErrorOrDefault() ?? "Failed to add user.";
                return;
            }

            Users.Add(newUser);
            SelectedUser = newUser;
            NewUserName = string.Empty;
            ErrorMessage = "User added successfully.";
        } catch (Exception ex) {
            _logger.LogError(ex, "Error adding user.");
            ErrorMessage = "An error occurred while adding the user.";
        } finally {
            IsValidating = false;
        }
    }
}

