using System;
using System.Security.Cryptography;
using System.Text;

namespace ChepoAPI.Services
{
    public interface IHashingService
    {
        string HashPassword(string password, string salt);
    }

    public static class HashingService
    {
        public static string HashPassword(string password, string salt)
        {
            // Combine the password and salt
            string saltedPassword = string.Concat(password, salt);

            // Compute the hash value
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}