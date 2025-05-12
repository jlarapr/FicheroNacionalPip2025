using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FicheroNacionalPip.Data.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [Required] 
        [StringLength(256)] 
        public string UserName { get; set; } = "";

        [Required] 
        public string PasswordHash { get; set; } = "";

        public string SecurityStamp { get; set; } = "";

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = "";

        [StringLength(1000)] 
        public string AreasDeAcceso { get; set; } = "";

        public bool? Locked { get; set; }

        public bool? Disable { get; set; }

        public bool? ForceChangePassword { get; set; }

        [StringLength(50)]
        public string StampDatePassword { get; set; }
    }
} 