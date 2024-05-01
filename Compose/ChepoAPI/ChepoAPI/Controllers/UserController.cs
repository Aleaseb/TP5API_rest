using ChepoAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using ChepoAPI.Services;

namespace ChepoAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public UsersController(PostgreDbContext context)
        {
            _context = context;
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
        [HttpGet("salt/{username}")]
        public async Task<ActionResult<string>> GetUserSalt(string username)
        {
            var user = await _context.users.FirstOrDefaultAsync(user => user.username == username);

            if (user == null)
            {
                return NotFound();
            }

            return user.salt;
        }
    }
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public AuthController(IConfiguration config, IUserService userService) { 
            _config = config;
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UsersDataAuth model)
        {
            byte[] HashedPassword = Encoding.Unicode.GetBytes(model.password);
            
            var user = _userService.Authenticate(model.username, HashedPassword);
            if (user == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.username),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30), // Token expiration time
            signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
            return CreatedAtAction("GiveToken", new { token = new JwtSecurityTokenHandler().WriteToken(token) }, new JwtSecurityTokenHandler().WriteToken(token));

        }

        [HttpGet("{token}")]
        public async Task<ActionResult<string>> GiveToken(string token)
        {
            return token;
        }
    }
}
