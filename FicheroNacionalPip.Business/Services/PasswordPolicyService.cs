using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Business.Services;

public sealed class PasswordPolicyService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    ILogger<PasswordPolicyService> logger)
    : IPasswordPolicyService {
    public event EventHandler<PasswordPolicyChangedEventArgs>? PolicyChanged;

    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly ILogger<PasswordPolicyService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    private void OnPolicyChanged(PasswordPolicyDto policy) {
        try {
            PolicyChanged?.Invoke(this, new PasswordPolicyChangedEventArgs(policy));
            _logger.LogInformation("Notificación de cambio de política enviada exitosamente");
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al notificar cambio de política");
        }
    }

    public void NotifyPolicyChanged(PasswordPolicyDto newPolicy) {
        OnPolicyChanged(newPolicy);
    }

    public async Task<Result<PasswordPolicyDto, string>> GetActivePasswordPolicyAsync() {
        try {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            PasswordPolicy? policy = await context.PasswordPolicies
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (policy == null) {
                _logger.LogWarning("No se encontró una política de contraseñas activa");
                return Result<PasswordPolicyDto, string>.Fail("No existe una política de contraseñas activa");
            }

            var dto = new PasswordPolicyDto {
                MaxPasswordAge = policy.MaxPasswordAge,
                MinPasswordLength = policy.MinPasswordLength,
                RequiredNumbers = policy.RequiredNumbers,
                RequiredNonAlphanumeric = policy.RequiredNonAlphanumeric,
                RequiredUppercase = policy.RequiredUppercase
            };

            return Result<PasswordPolicyDto, string>.Ok(dto);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al obtener la política de contraseñas activa");
            return Result<PasswordPolicyDto, string>.Fail($"Error al obtener la política de contraseñas: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> UpdatePasswordPolicyAsync(PasswordPolicyDto policyDto) {
        try {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();

            // Desactivar todas las políticas existentes
            List<PasswordPolicy> existingPolicies = await context.PasswordPolicies
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (PasswordPolicy existing in existingPolicies) {
                existing.IsActive = false;
                existing.ModifiedAt = DateTime.UtcNow;
            }

            // Crear nueva política
            var newPolicy = new PasswordPolicy {
                MaxPasswordAge = policyDto.MaxPasswordAge,
                MinPasswordLength = policyDto.MinPasswordLength,
                RequiredNumbers = policyDto.RequiredNumbers,
                RequiredNonAlphanumeric = policyDto.RequiredNonAlphanumeric,
                RequiredUppercase = policyDto.RequiredUppercase,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                IsActive = true
            };

            context.PasswordPolicies.Add(newPolicy);
            await context.SaveChangesAsync();

            _logger.LogInformation("Política de contraseñas actualizada exitosamente");
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al actualizar la política de contraseñas");
            return Result<bool, string>.Fail($"Error al actualizar la política de contraseñas: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> ValidatePasswordAsync(string password) {
        try {
            Result<PasswordPolicyDto, string> policyResult = await GetActivePasswordPolicyAsync();
            if (policyResult.IsFailure) {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault() ?? "Unknown error");
            }

            // PasswordPolicyDto? policy = policyResult.GetValueOrDefault();
            Result<IEnumerable<string>, string> validationErrors = await GetPasswordValidationErrorsAsync(password);

            if (validationErrors.IsSuccess && validationErrors.GetValueOrDefault()?.Any() == false) {
                return Result<bool, string>.Ok(true);
            }

            return Result<bool, string>.Fail(string.Join(", ", validationErrors.GetValueOrDefault() ?? []));
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al validar la contraseña");
            return Result<bool, string>.Fail($"Error al validar la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> IsPasswordExpiredAsync(string username) {
        try {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null) {
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            if (string.IsNullOrEmpty(user.StampDatePassword)) {
                return Result<bool, string>.Ok(true); // Forzar cambio si no hay fecha
            }

            Result<PasswordPolicyDto, string> policyResult = await GetActivePasswordPolicyAsync();
            if (policyResult.IsFailure) {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault() ?? "Unknown error");
            }

            PasswordPolicyDto? policy = policyResult.GetValueOrDefault();

            if (!DateTime.TryParse(user.StampDatePassword, out DateTime lastPasswordChange))
                return Result<bool, string>.Ok(true); // Forzar cambio si la fecha no es válida

            if (policy == null) {
                return Result<bool, string>.Fail("La política de contraseñas activa no está disponible");
            }

            double daysOld = (DateTime.UtcNow - lastPasswordChange).TotalDays;
            return Result<bool, string>.Ok(daysOld >= policy.MaxPasswordAge);

        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al verificar la expiración de la contraseña");
            return Result<bool, string>.Fail($"Error al verificar la expiración de la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<string>, string>> GetPasswordValidationErrorsAsync(string password) {
        try {
            var errors = new List<string>();
            Result<PasswordPolicyDto, string> policyResult = await GetActivePasswordPolicyAsync();

            if (policyResult.IsFailure) {
                return Result<IEnumerable<string>, string>.Fail(policyResult.GetErrorOrDefault()  ?? "Unknown error" );
            }

            PasswordPolicyDto? policy = policyResult.GetValueOrDefault();

            if (string.IsNullOrEmpty(password)) {
                errors.Add("La contraseña no puede estar vacía");
                return Result<IEnumerable<string>, string>.Ok(errors);
            }

            if (policy != null &&  password.Length < policy.MinPasswordLength) {
                errors.Add($"La contraseña debe tener al menos {policy.MinPasswordLength} caracteres");
            }

            int numbersCount = password.Count(char.IsDigit);
            if (policy != null && numbersCount < policy.RequiredNumbers) {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredNumbers} número(s)");
            }

            int uppercaseCount = password.Count(char.IsUpper);
            if (policy != null && uppercaseCount < policy.RequiredUppercase) {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredUppercase} mayúscula(s)");
            }

            int nonAlphanumericCount = password.Count(c => !char.IsLetterOrDigit(c));
            if (policy != null && nonAlphanumericCount < policy.RequiredNonAlphanumeric) {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredNonAlphanumeric} carácter(es) especial(es)");
            }

            return Result<IEnumerable<string>, string>.Ok(errors);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al validar los requisitos de la contraseña");
            return Result<IEnumerable<string>, string>.Fail($"Error al validar los requisitos de la contraseña: {ex.Message}");
        }
    }
}