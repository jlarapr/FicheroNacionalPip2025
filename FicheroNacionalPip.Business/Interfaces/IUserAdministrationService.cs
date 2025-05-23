using FicheroNacionalPip.Common;
using FicheroNacionalPip.Business.Models;

namespace FicheroNacionalPip.Business.Interfaces;

/// <summary>
/// Interface para la administración de usuarios.
/// </summary>
public interface IUserAdministrationService
{
    /// <summary>
    /// Añade un nuevo usuario al sistema.
    /// </summary>
    /// <param name="user">Información del usuario a añadir</param>
    /// <returns>Resultado indicando si la operación fue exitosa o un mensaje de error</returns>
    Task<Result<bool, string>> AddUserAsync(UserDto user);

    /// <summary>
    /// Edita la información de un usuario existente.
    /// </summary>
    /// <param name="user">Información actualizada del usuario</param>
    /// <returns>Resultado indicando si la operación fue exitosa o un mensaje de error</returns>
    Task<Result<bool, string>> EditUserAsync(UserDto user);

    /// <summary>
    /// Elimina un usuario del sistema.
    /// </summary>
    /// <param name="username">Nombre del usuario a eliminar</param>
    /// <returns>Resultado indicando si la operación fue exitosa o un mensaje de error</returns>
    Task<Result<bool, string>> DeleteUserAsync(string username);

    /// <summary>
    /// Obtiene todos los usuarios del sistema.
    /// </summary>
    /// <returns>Lista de usuarios como UserDto.</returns>
    Task<List<UserDto>> GetAllUsersAsync();
}
