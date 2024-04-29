using ChepoAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChepoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public ServerController(PostgreDbContext context)
        {
            _context = context;
        }

        // GET: api/Server
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServerData>>> GetServers()
        {
            return await _context.servers.ToListAsync();
        }

        // GET: api/Server/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerData>> GetServerData(Guid id)
        {
            var serverData = await _context.servers.FindAsync(id);

            if (serverData == null)
            {
                return NotFound();
            }

            return serverData;
        }

        // PUT: api/Server/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServerData(Guid id, string? name, string? ip)
        {
            ServerData? serverData = await _context.servers.FindAsync(id);

            if (serverData == null)
            {
                return NotFound();
            }

            if (id != serverData.uuid)
            {
                return BadRequest();
            }

            _context.Entry(serverData).State = EntityState.Modified;

            if (name != null)
            {
                serverData.name = name;
            }

            if (ip != null)
            {
                serverData.ip = ip;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServerDataExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Server
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ServerData>> PostServerData(string name, string ip)
        {
            ServerData serverData = new ServerData();
            serverData.uuid = Guid.NewGuid();
            serverData.name = name;
            serverData.ip = ip;
            _context.servers.Add(serverData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServerData", new { id = serverData.uuid }, serverData);
        }

        // DELETE: api/Server/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServerData(Guid id)
        {
            var serverData = await _context.servers.FindAsync(id);
            if (serverData == null)
            {
                return NotFound();
            }

            _context.servers.Remove(serverData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServerDataExists(Guid id)
        {
            return _context.servers.Any(e => e.uuid == id);
        }
    }
}
