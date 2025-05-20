using System.ComponentModel.DataAnnotations;

namespace FicheroNacionalPip.Data.Models;

public class PasswordPolicy {
    [Key]
    public int Id { get; set; }

    [Required]
    public int MaxPasswordAge { get; set; }

    [Required]
    public int MinPasswordLength { get; set; }

    [Required]
    public int RequiredNumbers { get; set; }

    [Required]
    public int RequiredNonAlphanumeric { get; set; }

    [Required]
    public int RequiredUppercase { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsActive { get; set; }
}