﻿using ChepoAPI;
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
            var servers = await _context.servers.ToListAsync();
            var playerstates = await _context.player_state.ToListAsync();

            if (servers == null || playerstates == null)
            {
                return BadRequest();
            }

            foreach (var server in servers)
            {
                server.nb_players = 0;
                foreach (var player in playerstates)
                {
                    if (player.server_uuid == server.uuid)
                    {
                        server.nb_players++;
                    }
                }
            }

            return Ok(servers);
        }

        // GET: api/Server/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServerData>> GetServerData(Guid id)
        {
            var serverData = await _context.servers.FindAsync(id);
            var playerstates = await _context.player_state.ToListAsync();

            if (playerstates == null)
            {
                return BadRequest();
            }

            if (serverData == null)
            {
                return NotFound();
            }

            serverData.nb_players = 0;
            foreach (var player in playerstates)
            {
                if (player.server_uuid == serverData.uuid)
                {
                    serverData.nb_players++;
                }
            }

            return serverData;
        }

        [HttpGet("mmr/{username}")]
        public async Task<ActionResult<ServerData>> GetMatchingServer(string username)
        {
            var servers = await _context.servers.ToListAsync();
            var playerstates = await _context.player_state.ToListAsync();

            if (servers == null || playerstates == null)
            {
                return BadRequest();
            }

            foreach (var server in servers)
            {
                server.nb_players = 0;
                foreach (var player in playerstates)
                {
                    if (player.server_uuid == server.uuid)
                    {
                        server.nb_players++;
                    }
                }
            }

            var user = await _context.users.FirstOrDefaultAsync(user => user.username == username);
            if (user == null)
            {
                return NotFound();
            }

            var userStats = await _context.player_stats.FindAsync(user.uuid);
            if (userStats == null)
            {
                return NotFound();
            }

            var userRank = await _context.ranks.FindAsync(userStats.rank_uuid);
            if (userRank == null)
            {
                return NotFound();
            }

            ServerData? bestServer = null;
            float closestMmrDistance = 50.0f;

            foreach (var server in servers)
            {
                if (server.avg_mmr == null)
                {
                    if (closestMmrDistance > 1.5f)
                    {
                        bestServer = server;
                    }
                }
                else
                {
                    float newMmrDistance = MathF.Abs(server.avg_mmr.Value - userRank.mmr_value);
                    if (newMmrDistance <= 1.5f && closestMmrDistance > newMmrDistance)
                    {
                        bestServer = server;
                        closestMmrDistance = newMmrDistance;
                    }
                }

            }

            if (bestServer == null)
            {
                return NotFound();
            }

            return Ok(bestServer);
        }

        // PUT: api/Server/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServerData(Guid id, string? name, string? ip, float? mmr)
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
            if (mmr != null)
            {
                serverData.avg_mmr = mmr;
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
            serverData.avg_mmr = null;
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
