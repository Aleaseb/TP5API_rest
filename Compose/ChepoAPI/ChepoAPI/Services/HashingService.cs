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
            
            //--------------- OLD SHA Conversion ---------------
            // Compute the hash value
            /*using (SHA512 sha512 = SHA512.Create())
            {
                byte[] hashedBytes = sha512.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
                //return saltedPassword;
            }*/
            //--------------------------------------------------
            
            var bytes = System.Text.Encoding.UTF8.GetBytes(saltedPassword);
            using (var hash = System.Security.Cryptography.SHA512.Create())
            {
                var hashedInputBytes = hash.ComputeHash(bytes);

                // Convert to text
                // StringBuilder Capacity is 128, because 512 bits / 8 bits in byte * 2 symbols for byte 
                var hashedInputStringBuilder = new System.Text.StringBuilder(128);
                foreach (var b in hashedInputBytes)
                    hashedInputStringBuilder.Append(b.ToString("x2"));
                return hashedInputStringBuilder.ToString();
            }
        }
    }
}