using Microsoft.EntityFrameworkCore;
using FicheroNacionalPip.Data.Models;

namespace FicheroNacionalPip.Data.DbContext.Seeds;

public static class DefaultPasswordPolicySeed
{
    public static void SeedDefaultPasswordPolicy(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PasswordPolicy>().HasData(
            new PasswordPolicy
            {
                Id = 1,
                MaxPasswordAge = 90,
                MinPasswordLength = 8,
                RequiredNumbers = 1,
                RequiredNonAlphanumeric = 1,
                RequiredUppercase = 1,
                CreatedAt = DateTime.Parse("2025-01-01"),  // Fecha fija para la semilla
                ModifiedAt = DateTime.Parse("2025-01-01"), // Fecha fija para la semilla
                IsActive = true
            }
        );
    }
} 