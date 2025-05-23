
using Microsoft.AspNetCore.Identity;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Business.Models;
using FicheroNacionalPip.Common;
using FicheroNacionalPip.Data.DbContext;
using FicheroNacionalPip.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FicheroNacionalPip.Business.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly PasswordHasher<User> _passwordHasher;
        private UserAuthInfo? _currentUser;

        public AuthenticationService(IDbContextFactory<ApplicationDbContext> dbContextFactory, ILogger<AuthenticationService> logger)
        {
            _dbContextFactory = dbContextFactory ?? throw new ArgumentNullException(nameof(dbContextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _passwordHasher = new PasswordHasher<User>();
        }

        public Result<UserAuthInfo, string> Authenticate(string username, string password)
        {
            try
            {
                _logger.LogInformation("Intento de autenticación para el usuario: {Username}", username);

                // Diagnóstico: Verificar variables de entorno
                string? keyEnv = Environment.GetEnvironmentVariable("FNP2025_KEY", EnvironmentVariableTarget.User);
                string? ivEnv = Environment.GetEnvironmentVariable("FNP2025_IV", EnvironmentVariableTarget.User);
                
                _logger.LogInformation("Variable FNP2025_KEY existe: {Exists}", !string.IsNullOrEmpty(keyEnv));
                _logger.LogInformation("Variable FNP2025_IV existe: {Exists}", !string.IsNullOrEmpty(ivEnv));

                if (string.IsNullOrEmpty(username))
                    return Result<UserAuthInfo, string>.Fail("El nombre de usuario no puede estar vacío.");

                if (string.IsNullOrEmpty(password))
                    return Result<UserAuthInfo, string>.Fail("La contraseña no puede estar vacía.");

                try {
                    // Crear y disponer de un contexto para diagnóstico
                    using ApplicationDbContext contextDiag = _dbContextFactory.CreateDbContext();
                    _logger.LogInformation("Conexión a la base de datos creada correctamente");
                    // Intentar realizar una operación simple para probar la conexión
                    bool usersExist = contextDiag.Users.Any();
                    _logger.LogInformation("Verificación de usuarios exitosa: {UsersExist}", usersExist);
                } catch (Exception exConn) {
                    _logger.LogError(exConn, "Error al conectar a la base de datos");
                    return Result<UserAuthInfo, string>.Fail($"Error de conexión a la base de datos: {exConn.Message}");
                }

                using ApplicationDbContext dbContext = _dbContextFactory.CreateDbContext();
                User? user = dbContext.Users
                    .FirstOrDefault(u => u.UserName.ToLower() == username.ToLower());

                if (user == null)
                {
                    _logger.LogWarning("Intento de autenticación fallido: Usuario no encontrado: {Username}", username);
                    return Result<UserAuthInfo, string>.Fail("Usuario o contraseña incorrectos.");
                }

                if (user.Locked == true)
                {
                    _logger.LogWarning("Intento de autenticación fallido: Usuario bloqueado: {Username}", username);
                    return Result<UserAuthInfo, string>.Fail("La cuenta está bloqueada. Contacte al administrador.");
                }

                if (user.Disable == true)
                {
                    _logger.LogWarning("Intento de autenticación fallido: Usuario deshabilitado: {Username}", username);
                    return Result<UserAuthInfo, string>.Fail("La cuenta está deshabilitada. Contacte al administrador.");
                }

                // Verificar la contraseña (aquí asumimos que se usa ASP.NET Identity para el hash)
                if (!VerifyPasswordHash(password, user.PasswordHash))
                {
                    _logger.LogWarning("Intento de autenticación fallido: Contraseña incorrecta para: {Username}", username);
                    return Result<UserAuthInfo, string>.Fail("Usuario o contraseña incorrectos.");
                }

                // Autenticación exitosa
                var userInfo = new UserAuthInfo
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Email = user.Email,
                    AreasDeAcceso = user.AreasDeAcceso,
                    ForceChangePassword = user.ForceChangePassword ?? false
                };

                _currentUser = userInfo;
                _logger.LogInformation("Autenticación exitosa para el usuario: {Username}", username);
                
                return Result<UserAuthInfo, string>.Ok(userInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario: {Username}", username);
                return Result<UserAuthInfo, string>.Fail($"Error durante la autenticación: {ex.Message}");
            }
        }

        public bool IsAuthenticated() => _currentUser != null;

        public void Logout()
        {
            if (_currentUser != null)
            {
                _logger.LogInformation("Cierre de sesión para el usuario: {Username}", _currentUser.UserName);
                _currentUser = null;
            }
        }

        public UserAuthInfo? GetCurrentUser() => _currentUser;

        // Método para verificar el hash de la contraseña (compatible con ASP.NET Identity)
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            try
            {
                var user = new User();

                // // Generar nuevo hash para comparar
                // var tempUser = new User();
                // var newHash = _passwordHasher.HashPassword(tempUser, password);
                // _logger.LogInformation("Nuevo hash generado: {NewHash}", newHash);

                var result = _passwordHasher.VerifyHashedPassword(user, storedHash, password);
                _logger.LogInformation("Resultado de verificación: {Result}", result);
                return result != PasswordVerificationResult.Failed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar el hash de la contraseña");
                return false;
            }
        }
    }
} 