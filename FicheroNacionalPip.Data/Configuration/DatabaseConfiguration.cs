using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using FicheroNacionalPip.Common;

namespace FicheroNacionalPip.Data.Configuration
{
    public class DatabaseConfiguration
    {
        private const string KEY_ENV_NAME = "FNP2025_KEY";
        private const string IV_ENV_NAME = "FNP2025_IV";

        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        private string _encryptedPassword;
        private string _decryptedPassword;
        
        public string Password
        {
            get => _encryptedPassword;
            set
            {
                try
                {
                    // Intentar desencriptar para ver si es una contraseña encriptada
                    _encryptedPassword = value;
                    _decryptedPassword = DecryptPassword(value);
                }
                catch
                {
                    // Si falla la desencriptación, asumimos que es una contraseña sin encriptar
                    _decryptedPassword = value;
                    _encryptedPassword = EncryptPassword(value);
                }
            }
        }

        public bool IntegratedSecurity { get; set; }
        public bool TrustServerCertificate { get; set; }

        public string GetConnectionString()
        {
            var baseConnectionString = IntegratedSecurity
                ? $"Server={Server};Database={Database};Trusted_Connection=True;"
                : $"Server={Server};Database={Database};User Id={Username};Password={_decryptedPassword};" +
                  "Persist Security Info=True;";

            return baseConnectionString + 
                   $"TrustServerCertificate={TrustServerCertificate};" +
                   "MultipleActiveResultSets=true;" +
                   "Encrypt=True;" +
                   "Connection Timeout=30;" +
                   "Application Name=FicheroNacionalPip2025_Tools;" +
                   "Pooling=true;Min Pool Size=1;Max Pool Size=100;";
        }

        private static string EncryptPassword(string password)
        {
            var keysResult = GetEncryptionKeys();
            if (keysResult.IsFailure)
                throw new InvalidOperationException(keysResult.GetErrorOrDefault());

            (byte[] key, byte[] iv) = keysResult.GetValueOrDefault();

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(password);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        private static string DecryptPassword(string encryptedPassword)
        {
            var keysResult = GetEncryptionKeys();
            if (keysResult.IsFailure)
                throw new InvalidOperationException(keysResult.GetErrorOrDefault());

            (byte[] key, byte[] iv) = keysResult.GetValueOrDefault();

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedPassword)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }

        private static Result<(byte[] key, byte[] iv), string> GetEncryptionKeys()
        {
            try
            {
                string? keyBase64 = Environment.GetEnvironmentVariable(KEY_ENV_NAME, EnvironmentVariableTarget.User);
                string? ivBase64 = Environment.GetEnvironmentVariable(IV_ENV_NAME, EnvironmentVariableTarget.User);

                if (string.IsNullOrEmpty(keyBase64))
                    return Result<(byte[] key, byte[] iv), string>.Fail(
                        $"La variable de ambiente {KEY_ENV_NAME} no está configurada. Use la herramienta FicheroNacionalPip.Tools para generarla.");

                if (string.IsNullOrEmpty(ivBase64))
                    return Result<(byte[] key, byte[] iv), string>.Fail(
                        $"La variable de ambiente {IV_ENV_NAME} no está configurada. Use la herramienta FicheroNacionalPip.Tools para generarla.");

                try
                {
                    byte[] key = Convert.FromBase64String(keyBase64);
                    byte[] iv = Convert.FromBase64String(ivBase64);

                    // Validar longitudes
                    if (key.Length != 32) // 256 bits
                        return Result<(byte[] key, byte[] iv), string>.Fail(
                            $"La clave en {KEY_ENV_NAME} no tiene la longitud correcta (debe ser 256 bits).");

                    if (iv.Length != 16) // 128 bits
                        return Result<(byte[] key, byte[] iv), string>.Fail(
                            $"El IV en {IV_ENV_NAME} no tiene la longitud correcta (debe ser 128 bits).");

                    return Result<(byte[] key, byte[] iv), string>.Ok((key, iv));
                }
                catch (FormatException)
                {
                    return Result<(byte[] key, byte[] iv), string>.Fail(
                        "Las variables de ambiente no contienen valores Base64 válidos.");
                }
            }
            catch (Exception ex)
            {
                return Result<(byte[] key, byte[] iv), string>.Fail(
                    $"Error al obtener las claves de encriptación: {ex.Message}");
            }
        }
    }
} 