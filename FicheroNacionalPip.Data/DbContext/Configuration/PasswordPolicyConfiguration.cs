using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Data.DbContext.Configuration;

public class PasswordPolicyConfiguration : IEntityTypeConfiguration<PasswordPolicy>
{
    public void Configure(EntityTypeBuilder<PasswordPolicy> builder)
    {
        // Configuración de la entidad
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.MaxPasswordAge)
            .IsRequired()
            .HasComment("Edad máxima de la contraseña en días");
        
        builder.Property(p => p.MinPasswordLength)
            .IsRequired()
            .HasComment("Longitud mínima de la contraseña");
        
        builder.Property(p => p.RequiredNumbers)
            .IsRequired()
            .HasComment("Cantidad de números requeridos");
        
        builder.Property(p => p.RequiredNonAlphanumeric)
            .IsRequired()
            .HasComment("Cantidad de caracteres especiales requeridos");
        
        builder.Property(p => p.RequiredUppercase)
            .IsRequired()
            .HasComment("Cantidad de mayúsculas requeridas");
        
        builder.Property(p => p.CreatedAt)
            .IsRequired()
            .HasComment("Fecha de creación");
        
        builder.Property(p => p.ModifiedAt)
            .IsRequired()
            .HasComment("Fecha de última modificación");
        
        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasComment("Indica si la política está activa");
    }
} 