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

namespace ChepoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly PostgreDbContext _context;
        private readonly ITokenService _tokenService;
        
        public UsersController(PostgreDbContext context, ITokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<UsersData>>> GetUsers()
        {
            return await _context.users.ToListAsync();
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<UsersData>> GetUser(string username)
        {
            var user = await _context.users.FirstOrDefaultAsync(user => user.username == username);

            if (user == null)
            {
                return NotFound();
            }

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
