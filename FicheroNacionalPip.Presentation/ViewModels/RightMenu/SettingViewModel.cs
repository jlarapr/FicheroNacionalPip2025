using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;

namespace FicheroNacionalPip.Presentation.ViewModels.RightMenu;

public partial class SettingViewModel : ObservableObject {
    [ObservableProperty] private string _myTitle;

    // Propiedades para política de contraseñas
    [ObservableProperty] private int _maxPasswordAge;
    [ObservableProperty] private int _minPasswordLength;
    [ObservableProperty] private int _requiredNumbers;
    [ObservableProperty] private int _requiredNonAlphanumeric;
    [ObservableProperty] private int _requiredUppercase;

    // Valores originales para comparación
    private int _originalMaxPasswordAge;
    private int _originalMinPasswordLength;
    private int _originalRequiredNumbers;
    private int _originalRequiredNonAlphanumeric;
    private int _originalRequiredUppercase;

    // Propiedades para manejo de estado y errores
    [ObservableProperty] private bool _isSaving;

    partial void OnIsSavingChanged(bool value) {
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty] private bool _isLoading;

    partial void OnIsLoadingChanged(bool value) {
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _successMessage = string.Empty;

    // Propiedades para la cadena de conexión
    [ObservableProperty] private string _connectionString = string.Empty;
    [ObservableProperty] private bool _isTestingConnection;

    private readonly IPasswordPolicyService _policyService;
    private readonly ILogger<SettingViewModel> _logger;
    private readonly IDbConfigurationService _dbConfigService;

    // Comandos para la gestión de políticas de contraseña
    public IAsyncRelayCommand SavePasswordPolicyCommand { get; }
    public IAsyncRelayCommand LoadPasswordPolicyCommand { get; }

    // Comandos para MaxPasswordAge
    public IRelayCommand IncrementMaxPasswordAgeCommand { get; }
    public IRelayCommand DecrementMaxPasswordAgeCommand { get; }

    // Comandos para RequiredNumbers
    public IRelayCommand IncrementRequiredNumbersCommand { get; }
    public IRelayCommand DecrementRequiredNumbersCommand { get; }

    // Comandos para RequiredUppercase
    public IRelayCommand IncrementRequiredUppercaseCommand { get; }
    public IRelayCommand DecrementRequiredUppercaseCommand { get; }

    // Comandos para RequiredNonAlphanumeric
    public IRelayCommand IncrementRequiredNonAlphanumericCommand { get; }
    public IRelayCommand DecrementRequiredNonAlphanumericCommand { get; }

    // Comandos para MinPasswordLength
    public IRelayCommand IncrementMinPasswordLengthCommand { get; }
    public IRelayCommand DecrementMinPasswordLengthCommand { get; }

    // Comando para probar la conexión
    public IAsyncRelayCommand TestConnectionCommand { get; }

    public SettingViewModel(
        IPasswordPolicyService policyService,
        IDbConfigurationService dbConfigService,
        ILogger<SettingViewModel> logger) {
        _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbConfigService = dbConfigService ?? throw new ArgumentNullException(nameof(dbConfigService));
        MyTitle = "Settings";

        SavePasswordPolicyCommand = new AsyncRelayCommand(SavePasswordPolicyAsync, CanSavePasswordPolicy);
        LoadPasswordPolicyCommand = new AsyncRelayCommand(LoadPasswordPolicyAsync);

        // Inicializar comandos de incremento/decremento
        IncrementMaxPasswordAgeCommand = new RelayCommand(IncrementMaxPasswordAge, CanIncrementMaxPasswordAge);
        DecrementMaxPasswordAgeCommand = new RelayCommand(DecrementMaxPasswordAge, CanDecrementMaxPasswordAge);

        IncrementRequiredNumbersCommand = new RelayCommand(IncrementRequiredNumbers, CanIncrementRequiredNumbers);
        DecrementRequiredNumbersCommand = new RelayCommand(DecrementRequiredNumbers, CanDecrementRequiredNumbers);

        IncrementRequiredUppercaseCommand = new RelayCommand(IncrementRequiredUppercase, CanIncrementRequiredUppercase);
        DecrementRequiredUppercaseCommand = new RelayCommand(DecrementRequiredUppercase, CanDecrementRequiredUppercase);

        IncrementRequiredNonAlphanumericCommand = new RelayCommand(IncrementRequiredNonAlphanumeric, CanIncrementRequiredNonAlphanumeric);
        DecrementRequiredNonAlphanumericCommand = new RelayCommand(DecrementRequiredNonAlphanumeric, CanDecrementRequiredNonAlphanumeric);

        IncrementMinPasswordLengthCommand = new RelayCommand(IncrementMinPasswordLength, CanIncrementMinPasswordLength);
        DecrementMinPasswordLengthCommand = new RelayCommand(DecrementMinPasswordLength, CanDecrementMinPasswordLength);
        // Inicializar comando de prueba de conexión
        TestConnectionCommand = new AsyncRelayCommand(TestConnectionAsync, CanTestConnection);
        // Cargar la política al inicializar
        LoadPasswordPolicyCommand.Execute(null);
        LoadConnectionString();
    }

    private void LoadConnectionString() {
        try {
            _logger.LogInformation("Cargando cadena de conexión");

            var result = _dbConfigService.GetConnectionString();
            if (result.IsSuccess) {
                // Enmascarar la contraseña en la cadena de conexión
                string? connectionString = result.GetValueOrDefault();

                IEnumerable<string> parts = (connectionString ?? string.Empty).Split(';')
                    .Select(part => { return part.Trim().StartsWith("Password=", StringComparison.OrdinalIgnoreCase) ? "Password=********" : part; });

                ConnectionString = string.Join(";", parts);
                _logger.LogInformation("Cadena de conexión cargada y enmascarada exitosamente");
            }
            else {
                ErrorMessage = "Error al cargar la cadena de conexión: " + result.GetErrorOrDefault();
                _logger.LogError("Error al cargar la cadena de conexión: {Error}", result.GetErrorOrDefault());
            }
        }
        catch (Exception ex) {
            ErrorMessage = "Error inesperado al cargar la cadena de conexión";
            _logger.LogError(ex, "Error inesperado al cargar la cadena de conexión");
        }
    }

    private async Task TestConnectionAsync() {
        try {
            IsTestingConnection = true;

            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            _logger.LogInformation("Probando conexión a la base de datos");

            var result = await Task.Run(() => _dbConfigService.TestConnection());
            if (result.IsSuccess) {
                SuccessMessage = "Conexión exitosa a la base de datos";
                _logger.LogInformation("Prueba de conexión exitosa");
            }
            else {
                ErrorMessage = "Error al conectar a la base de datos: " + result.GetErrorOrDefault();
                _logger.LogError("Error en prueba de conexión: {Error}", result.GetErrorOrDefault());
            }
        }
        catch (Exception ex) {
            ErrorMessage = "Error inesperado al probar la conexión";
            _logger.LogError(ex, "Error inesperado al probar la conexión");
        }
        finally {
            IsTestingConnection = false;
        }
    }

    private bool CanTestConnection() => !IsTestingConnection;

    // Métodos para MaxPasswordAge (1-365)
    private void IncrementMaxPasswordAge() {
        MaxPasswordAge = Math.Min(MaxPasswordAge + 1, 365);
        IncrementMaxPasswordAgeCommand.NotifyCanExecuteChanged();
        DecrementMaxPasswordAgeCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private void DecrementMaxPasswordAge() {
        MaxPasswordAge = Math.Max(MaxPasswordAge - 1, 1);
        IncrementMaxPasswordAgeCommand.NotifyCanExecuteChanged();
        DecrementMaxPasswordAgeCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private bool CanIncrementMaxPasswordAge() => MaxPasswordAge < 365;
    private bool CanDecrementMaxPasswordAge() => MaxPasswordAge > 1;

    // Métodos para RequiredNumbers (1-5)
    private void IncrementRequiredNumbers() {
        RequiredNumbers = Math.Min(RequiredNumbers + 1, 5);
        IncrementRequiredNumbersCommand.NotifyCanExecuteChanged();
        DecrementRequiredNumbersCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private void DecrementRequiredNumbers() {
        RequiredNumbers = Math.Max(RequiredNumbers - 1, 1);
        IncrementRequiredNumbersCommand.NotifyCanExecuteChanged();
        DecrementRequiredNumbersCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private bool CanIncrementRequiredNumbers() => RequiredNumbers < 5;
    private bool CanDecrementRequiredNumbers() => RequiredNumbers > 1;

    // Métodos para RequiredUppercase (1-5)
    private void IncrementRequiredUppercase() {
        RequiredUppercase = Math.Min(RequiredUppercase + 1, 5);
        IncrementRequiredUppercaseCommand.NotifyCanExecuteChanged();
        DecrementRequiredUppercaseCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private void DecrementRequiredUppercase() {
        RequiredUppercase = Math.Max(RequiredUppercase - 1, 1);
        IncrementRequiredUppercaseCommand.NotifyCanExecuteChanged();
        DecrementRequiredUppercaseCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private bool CanIncrementRequiredUppercase() => RequiredUppercase < 5;
    private bool CanDecrementRequiredUppercase() => RequiredUppercase > 1;

    // Métodos para RequiredNonAlphanumeric (1-5)
    private void IncrementRequiredNonAlphanumeric() {
        RequiredNonAlphanumeric = Math.Min(RequiredNonAlphanumeric + 1, 5);
        IncrementRequiredNonAlphanumericCommand.NotifyCanExecuteChanged();
        DecrementRequiredNonAlphanumericCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private void DecrementRequiredNonAlphanumeric() {
        RequiredNonAlphanumeric = Math.Max(RequiredNonAlphanumeric - 1, 1);
        IncrementRequiredNonAlphanumericCommand.NotifyCanExecuteChanged();
        DecrementRequiredNonAlphanumericCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private bool CanIncrementRequiredNonAlphanumeric() => RequiredNonAlphanumeric < 5;
    private bool CanDecrementRequiredNonAlphanumeric() => RequiredNonAlphanumeric > 1;

    // Métodos para MinPasswordLength (8-20)
    private void IncrementMinPasswordLength() {
        MinPasswordLength = Math.Min(MinPasswordLength + 1, 20);
        IncrementMinPasswordLengthCommand.NotifyCanExecuteChanged();
        DecrementMinPasswordLengthCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private void DecrementMinPasswordLength() {
        MinPasswordLength = Math.Max(MinPasswordLength - 1, 8);
        IncrementMinPasswordLengthCommand.NotifyCanExecuteChanged();
        DecrementMinPasswordLengthCommand.NotifyCanExecuteChanged();
        SavePasswordPolicyCommand.NotifyCanExecuteChanged();
    }

    private bool CanIncrementMinPasswordLength() => MinPasswordLength < 20;
    private bool CanDecrementMinPasswordLength() => MinPasswordLength > 8;

    private bool CanSavePasswordPolicy() {
        bool hasChanges = MaxPasswordAge != _originalMaxPasswordAge ||
                          MinPasswordLength != _originalMinPasswordLength ||
                          RequiredNumbers != _originalRequiredNumbers ||
                          RequiredNonAlphanumeric != _originalRequiredNonAlphanumeric ||
                          RequiredUppercase != _originalRequiredUppercase;

        return !IsSaving &&
               !IsLoading &&
               hasChanges &&
               MaxPasswordAge >= 1 && MaxPasswordAge <= 365 &&
               MinPasswordLength >= 8 && MinPasswordLength <= 20 &&
               RequiredNumbers >= 1 && RequiredNumbers <= 5 &&
               RequiredNonAlphanumeric >= 1 && RequiredNonAlphanumeric <= 5 &&
               RequiredUppercase >= 1 && RequiredUppercase <= 5;
    }

    private async Task LoadPasswordPolicyAsync() {
        try {
            IsLoading = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            _logger.LogInformation("Cargando política de contraseñas");

            Result<PasswordPolicyDto, string> result = await _policyService.GetActivePasswordPolicyAsync();
            if (result.IsSuccess) {
                PasswordPolicyDto? policy = result.GetValueOrDefault();

                // Actualizar valores actuales
                MaxPasswordAge = policy?.MaxPasswordAge ?? 0;
                MinPasswordLength = policy?.MinPasswordLength ?? 0;
                RequiredNumbers = policy?.RequiredNumbers ?? 0;
                RequiredNonAlphanumeric = policy?.RequiredNonAlphanumeric ?? 0;
                RequiredUppercase = policy?.RequiredUppercase ?? 0;

                // Guardar valores originales
                _originalMaxPasswordAge = policy?.MaxPasswordAge ?? 0;
                _originalMinPasswordLength = policy?.MinPasswordLength ?? 0;
                _originalRequiredNumbers = policy?.RequiredNumbers ?? 0;
                _originalRequiredNonAlphanumeric = policy?.RequiredNonAlphanumeric ?? 0;
                _originalRequiredUppercase = policy?.RequiredUppercase ?? 0;

                _logger.LogInformation("Política de contraseñas cargada exitosamente");
            }
            else {
                ErrorMessage = "Error al cargar la política de contraseñas: " + result.GetErrorOrDefault();
                _logger.LogError("Error al cargar la política de contraseñas: {Error}", result.GetErrorOrDefault());
            }
        }
        catch (Exception ex) {
            ErrorMessage = "Error inesperado al cargar la política de contraseñas";
            _logger.LogError(ex, "Error inesperado al cargar la política de contraseñas");
        }
        finally {
            IsLoading = false;
        }
    }

    private async Task SavePasswordPolicyAsync() {
        try {
            IsSaving = true;
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            _logger.LogInformation("Guardando política de contraseñas");

            var policy = new PasswordPolicyDto {
                MaxPasswordAge = MaxPasswordAge,
                MinPasswordLength = MinPasswordLength,
                RequiredNumbers = RequiredNumbers,
                RequiredNonAlphanumeric = RequiredNonAlphanumeric,
                RequiredUppercase = RequiredUppercase
            };

            var result = await _policyService.UpdatePasswordPolicyAsync(policy);
            if (result.IsSuccess) {
                // Notificar el cambio de política
                _policyService.NotifyPolicyChanged(policy);

                // Actualizar valores originales
                _originalMaxPasswordAge = MaxPasswordAge;
                _originalMinPasswordLength = MinPasswordLength;
                _originalRequiredNumbers = RequiredNumbers;
                _originalRequiredNonAlphanumeric = RequiredNonAlphanumeric;
                _originalRequiredUppercase = RequiredUppercase;

                SuccessMessage = "Política de contraseñas guardada exitosamente";
                _logger.LogInformation("Política de contraseñas guardada y notificada exitosamente");
            }
            else {
                ErrorMessage = "Error al guardar la política de contraseñas: " + result.GetErrorOrDefault();
                _logger.LogError("Error al guardar la política de contraseñas: {Error}", result.GetErrorOrDefault());
            }
        }
        catch (Exception ex) {
            ErrorMessage = "Error inesperado al guardar la política de contraseñas";
            _logger.LogError(ex, "Error inesperado al guardar la política de contraseñas");
        }
        finally {
            IsSaving = false;
            SavePasswordPolicyCommand.NotifyCanExecuteChanged();
        }
    }
}