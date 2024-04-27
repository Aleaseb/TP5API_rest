using ChepoAPI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChepoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
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

    }
}
