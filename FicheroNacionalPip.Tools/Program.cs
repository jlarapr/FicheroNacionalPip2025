using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using FicheroNacionalPip.Business.Services;
using Microsoft.Extensions.Configuration;
using FicheroNacionalPip.Business.Interfaces;
using FicheroNacionalPip.Data.Configuration;
using System.Linq;

namespace FicheroNacionalPip.Tools;

public class Program
{
    private const string KEY_ENV_NAME = "FNP2025_KEY";
    private const string IV_ENV_NAME = "FNP2025_IV";

    public static void Main(string[] args)
    {
        while (true)
        {
            try
            {
                ShowMenu();
                var option = Console.ReadLine();

                Console.Clear();
                Console.WriteLine("Herramienta de Encriptación FNP2025");
                Console.WriteLine("==================================\n");

                switch (option)
                {
                    case "1":
                        GenerateKeys();
                        break;
                    case "2":
                        EncryptPassword();
                        break;
                    case "3":
                        TestDatabaseConnection();
                        break;
                    case "4":
                        DecryptPassword();
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Opción no válida.");
                        break;
                }

                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError: {ex.Message}");
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                Console.Clear();
            }
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Herramienta de Encriptación FNP2025");
        Console.WriteLine("==================================\n");
        Console.WriteLine("1. Generar nuevas claves de encriptación");
        Console.WriteLine("2. Encriptar contraseña");
        Console.WriteLine("3. Probar conexión a la base de datos");
        Console.WriteLine("4. Desencriptar contraseña");
        Console.WriteLine("5. Salir\n");
        Console.Write("Seleccione una opción (1-5): ");
    }

    private static void GenerateKeys()
    {
        Console.WriteLine("Generando nuevas claves de encriptación...\n");

        using (Aes aes = Aes.Create())
        {
            string keyBase64 = Convert.ToBase64String(aes.Key);
            string ivBase64 = Convert.ToBase64String(aes.IV);

            Environment.SetEnvironmentVariable(KEY_ENV_NAME, keyBase64, EnvironmentVariableTarget.User);
            Environment.SetEnvironmentVariable(IV_ENV_NAME, ivBase64, EnvironmentVariableTarget.User);

            Console.WriteLine("Claves generadas y guardadas en variables de ambiente:");
            Console.WriteLine($"{KEY_ENV_NAME}: {keyBase64}");
            Console.WriteLine($"{IV_ENV_NAME}: {ivBase64}");
        }
    }

    private static void EncryptPassword()
    {
        Console.Write("Ingrese la contraseña a encriptar: ");
        var password = Console.ReadLine();

        if (string.IsNullOrEmpty(password))
        {
            Console.WriteLine("La contraseña no puede estar vacía.");
            return;
        }

        var config = new DatabaseConfiguration { Password = password };
        Console.WriteLine($"\nContraseña encriptada (Base64): {config.Password}");
    }

    private static void DecryptPassword()
    {
        Console.Write("Ingrese la contraseña encriptada (Base64): ");
        var encryptedPassword = Console.ReadLine();

        if (string.IsNullOrEmpty(encryptedPassword))
        {
            Console.WriteLine("La contraseña encriptada no puede estar vacía.");
            return;
        }

        try
        {
            var config = new DatabaseConfiguration();
            config.Password = encryptedPassword;
            var connectionString = config.GetConnectionString();
            var decryptedPassword = connectionString
                .Split(';')
                .FirstOrDefault(p => p.StartsWith("Password="))
                ?.Split('=')[1];

            Console.WriteLine($"\nContraseña original: {decryptedPassword}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError al desencriptar: {ex.Message}");
        }
    }

    private static void TestDatabaseConnection()
    {
        Console.WriteLine("Probando conexión a la base de datos...\n");
        
        var service = new DbConfigurationService();

        var result = service.TestConnection();

        if (result.IsFailure)
        {
            Console.WriteLine("\nError al conectar a la base de datos:");
            Console.WriteLine(result.GetErrorOrDefault());
        }
    }
}
