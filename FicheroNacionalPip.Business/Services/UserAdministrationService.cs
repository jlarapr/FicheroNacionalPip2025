using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Business.Services;

/// <summary>
/// Servicio para la administración de usuarios.
/// </summary>
public class UserAdministrationService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<UserAdministrationService> logger)
    : IUserAdministrationService {

    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
    private readonly ILogger<UserAdministrationService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Result<bool, string>> AddUserAsync(UserDto user)
    {
        try
        {
            // nose puede crear un usuario admin ya por defecto hay un usuario admin
            if (user.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Intento de crear el usuario admin: {Username}", user.UserName);
                return Result<bool, string>.Fail("El usuario admin ya existe");
            }

            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();

            if (await context.Users.AnyAsync(u => u.UserName.ToLower() == user.UserName.ToLower()))
            {
                _logger.LogWarning("El usuario ya existe: {Username}", user.UserName);
                return Result<bool, string>.Fail("El usuario ya existe");
            }

            var newUser = new User
            {
                UserName = user.UserName,
                PasswordHash = "hashed_password_placeholder", // Replace with actual hashing logic
                StampDatePassword = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                ForceChangePassword = user.ForceChangePassword,
                Locked = user.Locked,
                Disable = user.Disable
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            _logger.LogInformation("Usuario añadido exitosamente: {Username}", user.UserName);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al añadir usuario: {Username}", user.UserName);
            return Result<bool, string>.Fail($"Error al añadir el usuario: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> EditUserAsync(UserDto user)
    {
        try
        {
            // nose puede editar el usuario admin
            if (user.UserName.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Intento de editar el usuario admin: {Username}", user.UserName);
                return Result<bool, string>.Fail("No se puede editar el usuario admin");
            }

            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? existingUser = await context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == user.UserName.ToLower());

            if (existingUser == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Username}", user.UserName);
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            existingUser.PasswordHash = "hashed_password_placeholder"; // Replace with actual hashing logic
            existingUser.StampDatePassword = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            existingUser.ForceChangePassword = user.ForceChangePassword;
            existingUser.Locked = user.Locked;
            existingUser.Disable = user.Disable;

            await context.SaveChangesAsync();

            _logger.LogInformation("Usuario editado exitosamente: {Username}", user.UserName);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al editar usuario: {Username}", user.UserName);
            return Result<bool, string>.Fail($"Error al editar el usuario: {ex.Message}");
        }
    }

    public async Task<Result<bool, string>> DeleteUserAsync(string username)
    {
        try
        {
            // nose puede borrar el usuario admin
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Intento de eliminar el usuario admin: {Username}", username);
                return Result<bool, string>.Fail("No se puede eliminar el usuario admin");
            }

            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            User? user = await context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == username.ToLower());

            if (user == null)
            {
                _logger.LogWarning("Usuario no encontrado: {Username}", username);
                return Result<bool, string>.Fail("Usuario no encontrado");
            }

            context.Users.Remove(user);
            await context.SaveChangesAsync();

            _logger.LogInformation("Usuario eliminado exitosamente: {Username}", username);
            return Result<bool, string>.Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar usuario: {Username}", username);
            return Result<bool, string>.Fail($"Error al eliminar el usuario: {ex.Message}");
        }
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        try
        {
            await using ApplicationDbContext context = await _dbContextFactory.CreateDbContextAsync();
            List<UserDto> users = await context.Users
                .Select(u => new UserDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    Email = u.Email,
                    AreasDeAcceso = u.AreasDeAcceso,
                    ForceChangePassword = u.ForceChangePassword ?? false,
                    Locked = u.Locked,
                    Disable = u.Disable,
                    StampDatePassword = u.StampDatePassword
                })
                .ToListAsync();

            _logger.LogInformation("Usuarios obtenidos exitosamente: {Count}", users.Count);
            return users;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener usuarios");
            throw;
        }
    }
}
