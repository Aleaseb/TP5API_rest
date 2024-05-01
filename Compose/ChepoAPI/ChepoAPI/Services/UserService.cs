using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChepoAPI.Services
{
    public interface IUserService
    {
        UsersData Authenticate(string username, byte[] HashedPassword);
    }
    public class UserService: IUserService
    {
        private readonly PostgreDbContext _context;
        public UserService(PostgreDbContext context) {  _context = context; }

        public UsersData Authenticate(string username, byte[] HashedPassword)
        {
            UsersData user = _context.users.FirstOrDefault(u => u.username == username) ?? null;
            if (user == null)
                return null;
            
            // Dans l'idéal, le mot de psse serait déjà stocké hashé.
            if (!VerifyPasswordHash(HashedPassword, user.password, user.salt))
                return null; 

            return user;
        }
        private static bool VerifyPasswordHash(byte[] HashedPassword, string storedHash, string storedSalt)
        {
            // A ce niveau de sécurité, nous sommes conscient que cela n'est pas du tout le plus optimal.
            /*using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(storedHash));*/
            byte[] computedHash;
            using (var sha512 = SHA512.Create())
            {
                byte[] storedpsswd = Encoding.Unicode.GetBytes(storedHash);
                byte[] storedsaltbyte = Encoding.Unicode.GetBytes(storedSalt);
                byte[] concated = storedpsswd.Concat<byte>(storedsaltbyte).ToArray();
                computedHash = sha512.ComputeHash(concated);
                
            }
            if (HashedPassword.Length != computedHash.Length)
                return false;

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != HashedPassword[i])
                {
                    return false; // Hash mismatch, password incorrect
                }
            }

            return true; // Password is correct
        }
}
    }
