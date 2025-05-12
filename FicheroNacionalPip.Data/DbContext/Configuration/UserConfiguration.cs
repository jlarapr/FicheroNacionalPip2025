using FicheroNacionalPip.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FicheroNacionalPip.Data.DbContext.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.UserName)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.PasswordHash)
                .IsRequired();

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(256);

            builder.Property(x => x.AreasDeAcceso)
                .HasMaxLength(1000);

            builder.Property(x => x.StampDatePassword)
                .HasMaxLength(50);

            // Índices
            builder.HasIndex(x => x.UserName).IsUnique();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.UserId).IsUnique();
        }
    }
} 