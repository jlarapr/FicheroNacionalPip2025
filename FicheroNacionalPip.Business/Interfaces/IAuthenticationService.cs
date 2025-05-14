using FicheroNacionalPip.Common;
using FicheroNacionalPip.Business.Models;

namespace FicheroNacionalPip.Business.Interfaces
{
    /// <summary>
    /// Define los servicios de autenticación para la aplicación.
    /// </summary>
    public interface IAuthenticationService
    {
        /// <summary>
        /// Autentica un usuario con nombre de usuario y contraseña.
        /// </summary>
        /// <param name="username">Nombre de usuario</param>
        /// <param name="password">Contraseña</param>
        /// <returns>Resultado que contiene información del usuario autenticado o un mensaje de error</returns>
        Result<UserAuthInfo, string> Authenticate(string username, string password);
        
        /// <summary>
        /// Verifica si el usuario actual está autenticado.
        /// </summary>
        /// <returns>Verdadero si el usuario está autenticado, falso en caso contrario</returns>
        bool IsAuthenticated();
        
        /// <summary>
        /// Cierra la sesión del usuario actual.
        /// </summary>
        void Logout();
        
        /// <summary>
        /// Obtiene información del usuario actualmente autenticado.
        /// </summary>
        /// <returns>Información del usuario autenticado o null si no hay usuario autenticado</returns>
        UserAuthInfo? GetCurrentUser();
    }
} 