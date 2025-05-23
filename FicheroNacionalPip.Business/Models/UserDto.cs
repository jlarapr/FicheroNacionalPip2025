namespace FicheroNacionalPip.Business.Models;

/// <summary>
/// DTO para transferir datos de usuario entre capas.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identificador único del usuario.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Nombre de usuario.
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Correo electrónico del usuario.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Áreas de acceso asignadas al usuario.
    /// </summary>
    public string AreasDeAcceso { get; set; } = string.Empty;

    /// <summary>
    /// Indica si el usuario debe cambiar su contraseña.
    /// </summary>
    public bool ForceChangePassword { get; set; }

    /// <summary>
    /// Contraseña del usuario (solo para creación o edición).
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Indica si el usuario está bloqueado debido a múltiples intentos fallidos de inicio de sesión.
    /// </summary>
    public bool? Locked { get; set; }

    /// <summary>
    /// Indica si el usuario está deshabilitado por el administrador.
    /// </summary>
    public bool? Disable { get; set; }

    /// <summary>
    /// Fecha de la última actualización de la contraseña.
    /// </summary>
    public string? StampDatePassword { get; set; }
}
