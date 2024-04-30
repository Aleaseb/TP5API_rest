using System;
using System.Linq;
using System.Security.Cryptography;

namespace ChepoAPI.Services
{
    public interface IUserService
    {
        UsersData Authenticate(string username, string password);
    }
    public class UserService: IUserService
    {
        private readonly PostgreDbContext _context;
        public UserService(PostgreDbContext context) {  _context = context; }

        public UsersData Authenticate(string username, string password)
        {
            var user= _context.users.FirstOrDefault(u=> u.username == username);
            if (user != null && VerifyPasswordHash(password, user.password, user.salt))
            {
                return user; 
            }

            return null;
        }
        private static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt));

            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != storedHash[i])
                {
                    return false; // Hash mismatch, password incorrect
                }
            }

            return true; // Password is correct
        }
}
    }
