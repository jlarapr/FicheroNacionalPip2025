using FicheroNacionalPip.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FicheroNacionalPip.Data.DbContext.Seeds
{
    public static class DefaultUserSeed
    {
        public static void SeedDefaultUser(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    UserId = Guid.Parse("8e445865-a24d-4543-a6c6-9443d048cdb9"), // GUID fijo para referencia
                    UserName = "admin",
                    Email = "admin@ficheironacionalpip.com",
                    PasswordHash = "AQAAAAEAACcQAAAAEOkUKJUWWS7cfwzF3OCqEmVmJ7WU1LLZz7p+j8RA+na7+ycmCRjJiyVlBVQzRaiwog==", // Contraseña: Admin123!
                    SecurityStamp = "1712ef28-6211-43f1-a789-b4ed3101a946", // GUID fijo para referencia
                    AreasDeAcceso = "ALL",
                    Locked = false,
                    Disable = false,
                    ForceChangePassword = true,
                    StampDatePassword = "2025-05-12 23:43:32" // Fecha fija para referencia
                }
            );
        }
    }
} 