using FicheroNacionalPip.Common;
using FicheroNacionalPip.Business.Models;

namespace FicheroNacionalPip.Business.Interfaces;

public interface IUserManagementService
{
    /// <summary>
    /// Valida si la contraseña actual del usuario es correcta.
    /// </summary>
    /// <param name="username">Nombre del usuario</param>
    /// <param name="currentPassword">La contraseña actual a validar</param>
    /// <returns>Resultado indicando si la contraseña es válida o un mensaje de error</returns>
    Task<Result<bool, string>> ValidatePasswordAsync(string username, string currentPassword);

    /// <summary>
    /// Cambia la contraseña del usuario.
    /// </summary>
    /// <param name="username">Nombre del usuario</param>
    /// <param name="currentPassword">Contraseña actual</param>
    /// <param name="newPassword">Nueva contraseña</param>
    /// <returns>Resultado indicando si el cambio fue exitoso o un mensaje de error</returns>
    Task<Result<bool, string>> ChangePasswordAsync(string username, string currentPassword, string newPassword);

    /// <summary>
    /// Obtiene el estado actual de la contraseña del usuario.
    /// </summary>
    /// <param name="username">Nombre del usuario</param>
    /// <returns>Información sobre el estado de la contraseña</returns>
    Task<Result<UserPasswordInfo, string>> GetPasswordStatusAsync(string username);
} 