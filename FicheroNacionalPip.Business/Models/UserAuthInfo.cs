namespace FicheroNacionalPip.Business.Models
{
    /// <summary>
    /// Contiene información básica del usuario autenticado.
    /// </summary>
    public record UserAuthInfo
    {
        public Guid UserId { get; init; }
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string AreasDeAcceso { get; init; } = string.Empty;
        public bool ForceChangePassword { get; init; }
    }
} 