using System.Security.Cryptography;

namespace StudentManagement.Helpers
{
    public class PasswordHelper
    {

        public (string Salt, string HashedPassword) GenerateSaltAndHash(string password, int saltSize = 16, int iterations = 10000)
        {
            byte[] salt = GenerateSalt(saltSize);
            string hashedPassword = HashPassword(password, salt, iterations);

            return (Convert.ToBase64String(salt), hashedPassword);
        }

        private byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        private string HashPassword(string password, byte[] salt, int iterations)
        {
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash
                return Convert.ToBase64String(hash);
            }
        }

        public bool ValidatePassword(string inputPassword, string storedHashedPassword, string storedSalt, int iterations = 10000)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);
            string inputHashedPassword = HashPassword(inputPassword, saltBytes, iterations);

            return storedHashedPassword == inputHashedPassword;
        }

    }
}
