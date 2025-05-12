using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Data.Configuration;
using FicheroNacionalPip.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.RegularExpressions;

namespace FicheroNacionalPip.Business.Services
{
    public class DbConfigurationService : IDbConfigurationService
    {
        private readonly DatabaseConfiguration _configuration;

        public DbConfigurationService()
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.Development.json", optional: false);

            var config = configBuilder.Build();
            var dbConfig = config.GetSection("DatabaseConfig");

            _configuration = new DatabaseConfiguration
            {
                Server = dbConfig["Server"] ?? "",
                Database = dbConfig["Database"] ?? "",
                Username = dbConfig["Username"] ?? "",
                Password = dbConfig["Password"] ?? "",
                IntegratedSecurity = bool.Parse(dbConfig["IntegratedSecurity"] ?? "false"),
                TrustServerCertificate = bool.Parse(dbConfig["TrustServerCertificate"] ?? "true")
            };
        }

        public Result<string, string> GetConnectionString()
        {
            try
            {
                var connectionString = _configuration.GetConnectionString();
                return Result<string, string>.Ok(connectionString);
            }
            catch (Exception ex)
            {
                return Result<string, string>.Fail($"Error al obtener la cadena de conexión: {ex.Message}");
            }
        }

        public Result<bool, string> TestConnection()
        {
            try
            {
                var connectionStringResult = GetConnectionString();
                if (connectionStringResult.IsFailure)
                    return Result<bool, string>.Fail(connectionStringResult.GetErrorOrDefault());

                var connectionString = connectionStringResult.GetValueOrDefault();
                Console.WriteLine($"\nIntentando conectar con la siguiente configuración:");
                Console.WriteLine($"Servidor: {_configuration.Server}");
                Console.WriteLine($"Base de datos: {_configuration.Database}");
                Console.WriteLine($"Usuario: {_configuration.Username}");
                Console.WriteLine($"Contraseña: ********");
                Console.WriteLine($"Autenticación integrada: {_configuration.IntegratedSecurity}");
                Console.WriteLine($"TrustServerCertificate: {_configuration.TrustServerCertificate}\n");

                // Extraer y ocultar la contraseña de la cadena de conexión
                var maskedConnectionString = connectionString;
                var passwordPattern = @"Password=([^;]+)";
                maskedConnectionString = Regex.Replace(
                    maskedConnectionString,
                    passwordPattern,
                    "Password=********"
                );
                Console.WriteLine($"Cadena de conexión: {maskedConnectionString}\n");

                using (var connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        Console.WriteLine("Conexión exitosa a la base de datos.");
                        return Result<bool, string>.Ok(true);
                    }
                    catch (SqlException ex)
                    {
                        var error = $"Error de SQL al probar la conexión: {ex.Message}\n" +
                                  $"Número de error: {ex.Number}\n" +
                                  $"Estado: {ex.State}\n" +
                                  $"Procedimiento: {ex.Procedure}\n" +
                                  $"Línea: {ex.LineNumber}\n" +
                                  $"Servidor: {ex.Server}";
                        return Result<bool, string>.Fail(error);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<bool, string>.Fail($"Error al probar la conexión: {ex.Message}");
            }
        }

        public Result<bool, string> Initialize(string server, string database, bool integratedSecurity, string? username = null, string? password = null, bool trustServerCertificate = true)
        {
            try
            {
                if (string.IsNullOrEmpty(server))
                {
                    return Result<bool, string>.Fail("El servidor no puede estar vacío.");
                }

                if (string.IsNullOrEmpty(database))
                {
                    return Result<bool, string>.Fail("La base de datos no puede estar vacía.");
                }

                if (!integratedSecurity)
                {
                    if (string.IsNullOrEmpty(username))
                    {
                        return Result<bool, string>.Fail("El nombre de usuario es requerido cuando no se usa autenticación integrada.");
                    }

                    if (string.IsNullOrEmpty(password))
                    {
                        return Result<bool, string>.Fail("La contraseña es requerida cuando no se usa autenticación integrada.");
                    }
                }

                _configuration.Server = server;
                _configuration.Database = database;
                _configuration.IntegratedSecurity = integratedSecurity;
                _configuration.TrustServerCertificate = trustServerCertificate;

                if (!integratedSecurity)
                {
                    _configuration.Username = username ?? "";
                    _configuration.Password = password ?? "";
                }

                return Result<bool, string>.Ok(true);
            }
            catch (Exception ex)
            {
                return Result<bool, string>.Fail($"Error al inicializar la configuración de base de datos: {ex.Message}");
            }
        }
    }
}