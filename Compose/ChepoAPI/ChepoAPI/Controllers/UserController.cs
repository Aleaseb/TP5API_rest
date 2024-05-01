using ChepoAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ChepoAPI.Services;
using ChepoAPI.Interfaces;

namespace ChepoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly PostgreDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly ICacheService _cacheService;

        public UsersController(PostgreDbContext context, ITokenService tokenService, ICacheService cacheService)
        {
            _context = context;
            _tokenService = tokenService;
            _cacheService = cacheService;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<UsersData>>> GetUsers()
        {
            var cacheData = _cacheService.GetData<List<UsersData>>("users");
            if (cacheData != null)
            {
                return cacheData;
            }

            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            var dataToCache = await _context.users.ToListAsync();
            _cacheService.SetData("users", dataToCache, expirationTime);

            return dataToCache;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UsersData>> GetUser(string username)
        {
            var cacheData = _cacheService.GetData<UsersData>(username);
            if (cacheData != null)
            {
                return cacheData;
            }

            var user = await _context.users.FirstOrDefaultAsync(user => user.username == username);
            if (user == null)
            {
                return NotFound();
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
            _cacheService.SetData(username, user, expirationTime);
            return user;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(string username, string password)
        {
            var user = await _context.users.FirstOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                return NotFound("Utilisateur introuvable");
            }

            // Hash the provided password with the user's salt and compare it with the stored password
            var hashedPassword = HashingService.HashPassword(user.password, user.salt);

            if (password != hashedPassword)
            {
                return Unauthorized("Mot de passe incorrect" + hashedPassword);
            }

            // If the password is correct, generate JWT token
            var token = _tokenService.GenerateToken(user);

            return Ok(new
            {
                token
            });
        }

    }
}
