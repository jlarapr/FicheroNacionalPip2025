using FicheroNacionalPip.Business.Interfaces;
using Microsoft.Extensions.Configuration;
using FicheroNacionalPip.Common;

namespace FicheroNacionalPip.Business.Services
{
    public class ApplicationInitializer
    {
        private readonly IDbConfigurationService _dbConfigService;
        private readonly IConfiguration _configuration;

        public ApplicationInitializer(IDbConfigurationService dbConfigService, IConfiguration configuration)
        {
            _dbConfigService = dbConfigService ?? throw new ArgumentNullException(nameof(dbConfigService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public Result<bool, string> Initialize()
        {
            return InitializeDatabase();
        }

        private Result<bool, string> InitializeDatabase()
        {
            try
            {
                var dbConfig = _configuration.GetSection("DatabaseConfig");
                if (!dbConfig.Exists())
                {
                    return Result<bool, string>.Fail("La sección 'DatabaseConfig' no existe en la configuración.");
                }

                var server = dbConfig["Server"];
                var database = dbConfig["Database"];
                var integratedSecurityStr = dbConfig["IntegratedSecurity"] ?? "true";
                var trustServerCertificateStr = dbConfig["TrustServerCertificate"] ?? "true";

                if (!bool.TryParse(integratedSecurityStr, out bool integratedSecurity))
                {
                    return Result<bool, string>.Fail("El valor de IntegratedSecurity no es válido.");
                }

                if (!bool.TryParse(trustServerCertificateStr, out bool trustServerCertificate))
                {
                    return Result<bool, string>.Fail("El valor de TrustServerCertificate no es válido.");
                }

                string? username = null;
                string? password = null;

                if (!integratedSecurity)
                {
                    username = dbConfig["Username"];
                    password = dbConfig["Password"];
                }

                var initResult = _dbConfigService.Initialize(
                    server: server ?? "",
                    database: database ?? "",
                    integratedSecurity: integratedSecurity,
                    username: username,
                    password: password,
                    trustServerCertificate: trustServerCertificate
                );

                if (initResult.IsFailure)
                {
                    return Result<bool, string>.Fail($"Error al inicializar la base de datos: {initResult.GetErrorOrDefault()}");
                }

                var connectionStringResult = _dbConfigService.GetConnectionString();
                if (connectionStringResult.IsFailure)
                {
                    return Result<bool, string>.Fail($"Error al obtener la cadena de conexión: {connectionStringResult.GetErrorOrDefault()}");
                }

                return Result<bool, string>.Ok(true);
            }
            catch (Exception ex)
            {
                return Result<bool, string>.Fail($"Error inesperado al inicializar la base de datos: {ex.Message}");
            }
        }
    }
} 