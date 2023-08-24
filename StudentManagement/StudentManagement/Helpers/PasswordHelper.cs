using System.Security.Cryptography;

namespace StudentManagement.Helpers
{
    public class PasswordHelper
    {
        public byte[] GenerateSalt(int size)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
