using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Business.Services;

public class PasswordPolicyService : IPasswordPolicyService
{
    public event EventHandler<PasswordPolicyChangedEventArgs>? PolicyChanged;
    
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ILogger<PasswordPolicyService> _logger;

    public PasswordPolicyService(
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ILogger<PasswordPolicyService> logger)
    {
        _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected virtual void OnPolicyChanged(PasswordPolicyDto policy)
    {
        try
        {
            PolicyChanged?.Invoke(this, new PasswordPolicyChangedEventArgs(policy));
            _logger.LogInformation("Notificación de cambio de política enviada exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al notificar cambio de política");
        }
    }

    public void NotifyPolicyChanged(PasswordPolicyDto newPolicy)
    {
        OnPolicyChanged(newPolicy);
    }

    public async Task<Result<PasswordPolicyDto, string>> GetActivePasswordPolicyAsync()
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var policy = await context.PasswordPolicies
                .Where(p => p.IsActive)
                .OrderByDescending(p => p.Id)
                .FirstOrDefaultAsync();

            if (policy == null)
            {
                _logger.LogWarning("No se encontró una política de contraseñas activa");
                return Result<PasswordPolicyDto, string>.Fail("No existe una política de contraseñas activa");
            }

            var dto = new PasswordPolicyDto
            {
                MaxPasswordAge = policy.MaxPasswordAge,
                MinPasswordLength = policy.MinPasswordLength,
                RequiredNumbers = policy.RequiredNumbers,
                RequiredNonAlphanumeric = policy.RequiredNonAlphanumeric,
                RequiredUppercase = policy.RequiredUppercase
            };

            return Result<PasswordPolicyDto, string>.Ok(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la política de contraseñas activa");
            return Result<PasswordPolicyDto, string>.Fail($"Error al obtener la política de contraseñas: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> UpdatePasswordPolicyAsync(PasswordPolicyDto policyDto)
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            
            // Desactivar todas las políticas existentes
            var existingPolicies = await context.PasswordPolicies
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var existing in existingPolicies)
            {
                existing.IsActive = false;
                existing.ModifiedAt = DateTime.UtcNow;
            }

            // Crear nueva política
            var newPolicy = new PasswordPolicy
            {
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la política de contraseñas");
            return Result<bool, string>.Fail($"Error al actualizar la política de contraseñas: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> ValidatePasswordAsync(string password)
    {
        try
        {
            var policyResult = await GetActivePasswordPolicyAsync();
            if (policyResult.IsFailure)
            {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault());
            }

            var policy = policyResult.GetValueOrDefault();
            var validationErrors = await GetPasswordValidationErrorsAsync(password);

            if (validationErrors.IsSuccess && !validationErrors.GetValueOrDefault().Any())
            {
                return Result<bool, string>.Ok(true);
            }

            return Result<bool, string>.Fail(string.Join(", ", validationErrors.GetValueOrDefault()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar la contraseña");
            return Result<bool, string>.Fail($"Error al validar la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> IsPasswordExpiredAsync(string username)
    {
        try
        {
            using var context = await _dbContextFactory.CreateDbContextAsync();
            var user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);

            if (user == null)
            {
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            if (string.IsNullOrEmpty(user.StampDatePassword))
            {
                return Result<bool, string>.Ok(true); // Forzar cambio si no hay fecha
            }

            var policyResult = await GetActivePasswordPolicyAsync();
            if (policyResult.IsFailure)
            {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault());
            }

            var policy = policyResult.GetValueOrDefault();
            
            if (DateTime.TryParse(user.StampDatePassword, out DateTime lastPasswordChange))
            {
                var daysOld = (DateTime.UtcNow - lastPasswordChange).TotalDays;
                return Result<bool, string>.Ok(daysOld >= policy.MaxPasswordAge);
            }

            return Result<bool, string>.Ok(true); // Forzar cambio si la fecha no es válida
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar la expiración de la contraseña");
            return Result<bool, string>.Fail($"Error al verificar la expiración de la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<IEnumerable<string>, string>> GetPasswordValidationErrorsAsync(string password)
    {
        try
        {
            var errors = new List<string>();
            var policyResult = await GetActivePasswordPolicyAsync();
            
            if (policyResult.IsFailure)
            {
                return Result<IEnumerable<string>, string>.Fail(policyResult.GetErrorOrDefault());
            }

            var policy = policyResult.GetValueOrDefault();

            if (string.IsNullOrEmpty(password))
            {
                errors.Add("La contraseña no puede estar vacía");
                return Result<IEnumerable<string>, string>.Ok(errors);
            }

            if (password.Length < policy.MinPasswordLength)
            {
                errors.Add($"La contraseña debe tener al menos {policy.MinPasswordLength} caracteres");
            }

            var numbersCount = password.Count(char.IsDigit);
            if (numbersCount < policy.RequiredNumbers)
            {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredNumbers} número(s)");
            }

            var uppercaseCount = password.Count(char.IsUpper);
            if (uppercaseCount < policy.RequiredUppercase)
            {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredUppercase} mayúscula(s)");
            }

            var nonAlphanumericCount = password.Count(c => !char.IsLetterOrDigit(c));
            if (nonAlphanumericCount < policy.RequiredNonAlphanumeric)
            {
                errors.Add($"La contraseña debe contener al menos {policy.RequiredNonAlphanumeric} carácter(es) especial(es)");
            }

            return Result<IEnumerable<string>, string>.Ok(errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al validar los requisitos de la contraseña");
            return Result<IEnumerable<string>, string>.Fail($"Error al validar los requisitos de la contraseña: {ex.Message}");
        }
    }
} 