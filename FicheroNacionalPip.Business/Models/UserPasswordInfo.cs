namespace FicheroNacionalPip.Business.Models;

/// <summary>
/// Contiene información sobre el estado de la contraseña de un usuario.
/// </summary>
public class UserPasswordInfo
{
    /// <summary>
    /// Fecha del último cambio de contraseña
    /// </summary>
    public DateTime? LastChanged { get; set; }

    /// <summary>
    /// Indica si se debe forzar el cambio de contraseña
    /// </summary>
    public bool ForceChange { get; set; }

    /// <summary>
    /// Indica si la contraseña ha expirado según la política actual
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Días restantes antes de que expire la contraseña
    /// </summary>
    public int DaysUntilExpiration { get; set; }
} 