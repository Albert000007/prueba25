using BaylongoApi.Services.Interfaces;
using System.Security.Cryptography;

namespace BaylongoApi.Services
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            // Parámetros de configuración (pueden ir en appsettings.json)
            const int iterations = 10000;
            const int hashSize = 32; // 256 bits
            const int saltSize = 16; // 128 bits

            // Generar salt aleatorio
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[saltSize]);

            // Generar el hash
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations);
            byte[] hash = pbkdf2.GetBytes(hashSize);

            // Combinar salt y hash
            byte[] hashBytes = new byte[saltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, saltSize);
            Array.Copy(hash, 0, hashBytes, saltSize, hashSize);

            // Convertir a Base64 para almacenamiento
            return Convert.ToBase64String(hashBytes);
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            // Convertir de Base64 a bytes
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extraer el salt (primeros 16 bytes)
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Calcular el hash de la contraseña proporcionada
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(32); // 256 bits

            // Comparar los hashes
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }
            return true;
        }
    }
}
