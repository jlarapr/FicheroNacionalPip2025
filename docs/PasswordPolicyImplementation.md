# Implementación de Políticas de Contraseña - Fichero Nacional PIP 2025

## 1. Modelo de Datos ✅

### 1.1 Nueva Tabla: PasswordPolicy ✅
```sql
CREATE TABLE PasswordPolicy (
    Id INT PRIMARY KEY IDENTITY(1,1),
    MaxPasswordAge INT NOT NULL,
    MinPasswordLength INT NOT NULL,
    RequiredNumbers INT NOT NULL,
    RequiredNonAlphanumeric INT NOT NULL,
    RequiredUppercase INT NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    ModifiedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IsActive BIT NOT NULL DEFAULT 1
)
```

### 1.2 Modelo en C# ✅
```csharp
// FicheroNacionalPip.Data.Models.PasswordPolicy
public class PasswordPolicy
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int MaxPasswordAge { get; set; }
    
    [Required]
    public int MinPasswordLength { get; set; }
    
    [Required]
    public int RequiredNumbers { get; set; }
    
    [Required]
    public int RequiredNonAlphanumeric { get; set; }
    
    [Required]
    public int RequiredUppercase { get; set; }
    
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsActive { get; set; }
}
```

## 2. Servicios

### 2.1 Interface IPasswordPolicyService ✅
```csharp
// FicheroNacionalPip.Business.Interfaces.IPasswordPolicyService
public interface IPasswordPolicyService
{
    Task<Result<PasswordPolicyDto, string>> GetActivePasswordPolicyAsync();
    Task<Result<bool, string>> UpdatePasswordPolicyAsync(PasswordPolicyDto policy);
    Task<Result<bool, string>> ValidatePasswordAsync(string password);
    Task<Result<bool, string>> IsPasswordExpiredAsync(string username);
    Task<Result<IEnumerable<string>, string>> GetPasswordValidationErrorsAsync(string password);
}
```

### 2.2 Implementación PasswordPolicyService ✅
- Implementar lógica de validación de contraseñas ✅
- Gestionar política activa ✅
- Validar expiración de contraseñas ✅
- Integrar con el servicio de autenticación existente ✅

## 3. ViewModels

### 3.1 Modificaciones al SettingViewModel ⏳
```csharp
// FicheroNacionalPip.Presentation.ViewModels.RightMenu.SettingViewModel
public partial class SettingViewModel : ObservableObject
{
    // Propiedades existentes...

    // Nuevas propiedades para política de contraseñas
    [ObservableProperty] private int _maxPasswordAge;
    [ObservableProperty] private int _minPasswordLength;
    [ObservableProperty] private int _requiredNumbers;
    [ObservableProperty] private int _requiredNonAlphanumeric;
    [ObservableProperty] private int _requiredUppercase;
    
    private readonly IPasswordPolicyService _policyService;
    
    // Comandos para la gestión de políticas de contraseña
    public IAsyncRelayCommand SavePasswordPolicyCommand { get; }
    public IAsyncRelayCommand LoadPasswordPolicyCommand { get; }
}
```

### 3.2 Modificaciones a LoginViewModel y ChangePasswordViewModel ⏳
- Integrar validación de políticas de contraseña
- Mostrar errores de validación específicos
- Verificar expiración de contraseñas

## 4. Vistas ⏳

### 4.1 Actualizar SettingWindow.xaml
- Implementar bindings para los controles existentes
- Agregar validación visual
- Mostrar feedback al usuario

### 4.2 Modificar LoginWindow.xaml y ChangePasswordWindow.xaml
- Agregar indicadores de requisitos de contraseña
- Mostrar estado de validación en tiempo real
- Implementar feedback visual de fortaleza de contraseña

## 5. Migración y Datos Iniciales

### 5.1 Nueva Migración ✅
```csharp
public partial class AddPasswordPolicy : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Crear tabla PasswordPolicy ✅
        // Insertar política por defecto ✅
    }
    
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Rollback logic ✅
    }
}
```

### 5.2 Datos Semilla ✅
```sql
INSERT INTO PasswordPolicy (
    MaxPasswordAge,
    MinPasswordLength,
    RequiredNumbers,
    RequiredNonAlphanumeric,
    RequiredUppercase,
    IsActive
) VALUES (
    90,  -- 90 días máximo
    8,   -- 8 caracteres mínimo
    1,   -- Al menos 1 número
    1,   -- Al menos 1 carácter especial
    1,   -- Al menos 1 mayúscula
    1    -- Activa
)
```

## 6. Pruebas ⏳

### 6.1 Pruebas Unitarias
- Validación de contraseñas
- Expiración de contraseñas
- Actualización de políticas

### 6.2 Pruebas de Integración
- Flujo completo de cambio de contraseña
- Validación en el login
- Persistencia de políticas

## 7. Documentación ⏳

### 7.1 Documentación Técnica
- Diagrama de clases
- Flujo de validación
- Integración con sistema existente

### 7.2 Documentación de Usuario
- Guía de requisitos de contraseña
- Procedimientos de cambio de contraseña
- FAQ de problemas comunes

## 8. Tareas de Implementación

1. [✅] Crear modelo y migración de base de datos
2. [✅] Implementar IPasswordPolicyService
3. [⏳] Desarrollar PasswordPolicyViewModel
4. [⏳] Actualizar vistas existentes
5. [✅] Integrar con sistema de autenticación
6. [⏳] Implementar pruebas unitarias
7. [⏳] Realizar pruebas de integración
8. [⏳] Documentar cambios y nuevas funcionalidades
9. [⏳] Revisar y actualizar mensajes de error
10. [✅] Implementar logging de eventos de política de contraseñas

## 9. Consideraciones de Seguridad ⏳

- Almacenamiento seguro de políticas
- Logging de cambios en políticas
- Auditoría de cambios de contraseña
- Protección contra ataques de fuerza bruta
- Manejo seguro de contraseñas en memoria

## 10. Mejoras Futuras

- Historial de contraseñas usadas
- Políticas por grupo de usuarios
- Notificaciones de expiración próxima
- Métricas de seguridad de contraseñas
- Integración con directivas de Active Directory 