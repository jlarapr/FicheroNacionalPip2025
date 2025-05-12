using FicheroNacionalPip.Common;

namespace FicheroNacionalPip.Business.Interfaces
{
    public interface IDbConfigurationService
    {
        Result<string, string> GetConnectionString();
        Result<bool, string> Initialize(string server, string database, bool integratedSecurity, string? username = null, string? password = null, bool trustServerCertificate = true);
        Result<bool, string> TestConnection();
    }
} 