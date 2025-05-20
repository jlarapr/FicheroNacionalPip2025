using System.ComponentModel.DataAnnotations;

namespace FicheroNacionalPip.Business.Models;

/// <summary>
/// DTO para transferir datos de política de contraseñas entre capas
/// </summary>
public class PasswordPolicyDto
{
    [Range(1, 365, ErrorMessage = "La edad máxima de la contraseña debe estar entre 1 y 365 días")]
    public int MaxPasswordAge { get; set; }
    
    [Range(8, 20, ErrorMessage = "La longitud mínima debe estar entre 8 y 20 caracteres")]
    public int MinPasswordLength { get; set; }
    
    [Range(1, 5, ErrorMessage = "El número de dígitos requeridos debe estar entre 1 y 5")]
    public int RequiredNumbers { get; set; }
    
    [Range(1, 5, ErrorMessage = "El número de caracteres especiales debe estar entre 1 y 5")]
    public int RequiredNonAlphanumeric { get; set; }
    
    [Range(1, 5, ErrorMessage = "El número de mayúsculas requeridas debe estar entre 1 y 5")]
    public int RequiredUppercase { get; set; }
} 