using System.Security.Cryptography;

namespace CafeDb.Utils
{
    public static class PasswordHasher
    {
        public static string Hash(string password)
        {
            using RandomNumberGenerator rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);

            using Rfc2898DeriveBytes pbkdf2 = new(password , salt, 100_000,HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(32);

            byte[] result = new byte[1 + salt.Length + hash.Length];
            result[0] = 0x01;
            Buffer.BlockCopy(salt , 0 , result, 1, salt.Length);
            Buffer.BlockCopy(hash , 0 , result , 1 + salt.Length , hash.Length);
            return Convert.ToBase64String(result);
        }


        public static bool Verify(string password, string storedHash)
        {
            var bytes = Convert.FromBase64String(storedHash);
            if (bytes[0] != 0x01) return false;

            var salt = new byte[16];
            Buffer.BlockCopy(bytes, 1, salt, 0, salt.Length);
            var stored = new byte[32];
            Buffer.BlockCopy(bytes, 1 + salt.Length, stored, 0, stored.Length);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100_000, HashAlgorithmName.SHA256);
            var computed = pbkdf2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(computed, stored);

        }
    }
}