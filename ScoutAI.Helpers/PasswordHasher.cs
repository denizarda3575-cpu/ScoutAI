using System;
using System.Security.Cryptography;

namespace ScoutAI.Helpers
{
    public static class PasswordHasher
    {
        public static class Pbkdf2
        {
            public static byte[] GetBytes(string password, byte[] salt, int iterations, int cb)
            {
                using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
                {
                    return deriveBytes.GetBytes(cb);
                }
            }
        }

        public class PasswordHashResult
        {
            public string Hash { get; set; }
            public string Salt { get; set; }
            public int Iterations { get; set; }
        }

        public static PasswordHashResult HashPassword(string password)
        {
            int iterations = 10000;
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            byte[] hash = Pbkdf2.GetBytes(password, salt, iterations, 32);

            return new PasswordHashResult
            {
                Hash = Convert.ToBase64String(hash),
                Salt = Convert.ToBase64String(salt),
                Iterations = iterations
            };
        }

        public static bool Verify(string password, string storedHash, string storedSalt, int iterations)
        {
            if (string.IsNullOrEmpty(storedSalt) || string.IsNullOrEmpty(storedHash)) return false;

            try
            {
                byte[] salt = Convert.FromBase64String(storedSalt);
                byte[] storedHashBytes = Convert.FromBase64String(storedHash);
                byte[] computedHash = Pbkdf2.GetBytes(password, salt, iterations, storedHashBytes.Length);

                if (computedHash.Length != storedHashBytes.Length) return false;

                int diff = 0;
                for (int i = 0; i < computedHash.Length; i++)
                {
                    diff |= computedHash[i] ^ storedHashBytes[i];
                }
                return diff == 0;
            }
            catch
            {
                return false;
            }
        }

        public static string GenerateRandomPassword()
        {
            string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lower = "abcdefghijklmnopqrstuvwxyz";
            string digits = "1234567890";
            string specials = "@#$!%*?&";
            string allChars = upper + lower + digits + specials;

            char[] pass = new char[10];
            using (var rng = RandomNumberGenerator.Create())
            {
                // Güvenlik politikası gereği her kategoriden en az 1 karakter garanti ediyoruz
                pass[0] = upper[GetSecureRandomInt(rng, upper.Length)];
                pass[1] = lower[GetSecureRandomInt(rng, lower.Length)];
                pass[2] = digits[GetSecureRandomInt(rng, digits.Length)];
                pass[3] = specials[GetSecureRandomInt(rng, specials.Length)];

                for (int i = 4; i < 10; i++)
                {
                    pass[i] = allChars[GetSecureRandomInt(rng, allChars.Length)];
                }

                // Karakterlerin yerini tamamen karıştırıyoruz (Fisher-Yates Shuffle)
                for (int i = pass.Length - 1; i > 0; i--)
                {
                    int j = GetSecureRandomInt(rng, i + 1);
                    char temp = pass[i];
                    pass[i] = pass[j];
                    pass[j] = temp;
                }
            }

            return new string(pass);
        }

        private static int GetSecureRandomInt(RandomNumberGenerator rng, int max)
        {
            byte[] data = new byte[4];
            rng.GetBytes(data);
            uint value = BitConverter.ToUInt32(data, 0);
            return (int)(value % (uint)max);
        }
    }
}