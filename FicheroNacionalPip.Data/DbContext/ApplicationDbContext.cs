using FicheroNacionalPip.Data.DbContext.Configuration;
using FicheroNacionalPip.Data.DbContext.Seeds;
using FicheroNacionalPip.Data.Models;
using FicheroNacionalPip.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FicheroNacionalPip.Data.DbContext
{
    public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordPolicy> PasswordPolicies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Aplicar configuraciones
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new PasswordPolicyConfiguration());
            
            // Agregar datos semilla
            DefaultUserSeed.SeedDefaultUser(modelBuilder);
            DefaultPasswordPolicySeed.SeedDefaultPasswordPolicy(modelBuilder);

            // Configuración de índices únicos para User
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserId)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // Configuración para las migraciones, lee desde los archivos del proyecto de Presentation
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../FicheroNacionalPip.Presentation"))
                    .AddJsonFile("appsettings.json", optional: false)
                    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                    .Build();

                var dbConfig = new DatabaseConfiguration();
                configuration.GetSection("DatabaseConfig").Bind(dbConfig);

                optionsBuilder.UseSqlServer(dbConfig.GetConnectionString());
            }
        }
    }
} 