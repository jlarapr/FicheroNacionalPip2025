using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Business.Services;

public class UserManagementService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    IPasswordPolicyService policyService,
    ILogger<UserManagementService> logger)
    : IUserManagementService {
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly IPasswordPolicyService _policyService = policyService ?? throw new ArgumentNullException(nameof(policyService));
    private readonly ILogger<UserManagementService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly PasswordHasher<User> _passwordHasher = new();

    public async Task<Result<bool, string>> ValidatePasswordAsync(string username, string currentPassword) {
        try {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null) {
                _logger.LogWarning("Usuario no encontrado: {Username}", username);
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            if (!VerifyPasswordHash(currentPassword, user.PasswordHash)) {
                _logger.LogWarning("Contraseña inválida para usuario: {Username}", username);
                return Result<bool, string>.Fail("La contraseña es incorrecta");
            }

            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al validar contraseña para usuario: {Username}", username);
            return Result<bool, string>.Fail($"Error al validar la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> ChangePasswordAsync(string username, string currentPassword, string newPassword) {
        try {
            // Validar contraseña actual
            Result<bool, string> validationResult = await ValidatePasswordAsync(username, currentPassword);
            if (!validationResult.IsSuccess) {
                return validationResult;
            }

            // Validar nueva contraseña contra política
            Result<bool, string> policyResult = await _policyService.ValidatePasswordAsync(newPassword);
            if (!policyResult.IsSuccess) {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault() ?? "Unknown error");
            }

            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null) {
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            // Actualizar contraseña
            user.PasswordHash = HashPassword(newPassword);
            user.StampDatePassword = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            user.ForceChangePassword = false;

            await context.SaveChangesAsync();

            _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Username}", username);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario: {Username}", username);
            return Result<bool, string>.Fail($"Error al cambiar la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> ChangePasswordAsync(string username, string newPassword) {
        try {
            // Validar nueva contraseña contra política
            Result<bool, string> policyResult = await _policyService.ValidatePasswordAsync(newPassword);
            if (!policyResult.IsSuccess) {
                return Result<bool, string>.Fail(policyResult.GetErrorOrDefault() ?? "Unknown error");
            }

            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null) {
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            // Actualizar contraseña
            user.PasswordHash = HashPassword(newPassword);
            user.StampDatePassword = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            user.ForceChangePassword = false;

            await context.SaveChangesAsync();

            _logger.LogInformation("Contraseña cambiada exitosamente para usuario: {Username}", username);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al cambiar contraseña para usuario: {Username}", username);
            return Result<bool, string>.Fail($"Error al cambiar la contraseña: {ex.Message}");
        }
    }

    public async Task<Result<UserPasswordInfo, string>> GetPasswordStatusAsync(string username) {
        try {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null) {
                return Result<UserPasswordInfo, string>.Fail("Usuario no encontrado");
            }

            Result<PasswordPolicyDto, string> policyResult = await _policyService.GetActivePasswordPolicyAsync();
            if (policyResult.IsFailure) {
                return Result<UserPasswordInfo, string>.Fail(policyResult.GetErrorOrDefault() ?? "Unknown error");
            }

            PasswordPolicyDto? policy = policyResult.GetValueOrDefault();
            var info = new UserPasswordInfo {
                LastChanged = DateTime.TryParse(user.StampDatePassword, out var date) ? date : null,
                ForceChange = user.ForceChangePassword ?? false,
                IsExpired = await _policyService.IsPasswordExpiredAsync(username)
                    .ContinueWith(t => t.Result.IsSuccess && t.Result.GetValueOrDefault())
            };

            // Calcular días hasta expiración
            if (info.LastChanged.HasValue) {
                double daysOld = (DateTime.UtcNow - info.LastChanged.Value).TotalDays;
                info.DaysUntilExpiration = Math.Max(0, policy?.MaxPasswordAge - (int)daysOld ?? 0);
            }
            else {
                info.DaysUntilExpiration = 0;
            }

            return Result<UserPasswordInfo, string>.Ok(info);
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al obtener estado de contraseña para usuario: {Username}", username);
            return Result<UserPasswordInfo, string>.Fail($"Error al obtener estado de la contraseña: {ex.Message}");
        }
    }

    private bool VerifyPasswordHash(string password, string storedHash) {
        try {
            var user = new User();
            var result = _passwordHasher.VerifyHashedPassword(user, storedHash, password);
            return result != PasswordVerificationResult.Failed;
        }
        catch (Exception ex) {
            _logger.LogError(ex, "Error al verificar el hash de la contraseña");
            return false;
        }
    }

    private string HashPassword(string password) {
        var user = new User();
        return _passwordHasher.HashPassword(user, password);
    }
}