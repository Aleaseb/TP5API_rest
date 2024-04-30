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
    public class PlayerStatesController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public PlayerStatesController(PostgreDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerStates
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStateData>>> GetPlayerStates()
        {
            return await _context.player_state.ToListAsync();
        }

        // GET: api/PlayerStates/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStateData>> GetPlayerStateData(Guid id)
        {
            var playerStateData = await _context.player_state.FindAsync(id);

            if (playerStateData == null)
            {
                return NotFound();
            }

            return playerStateData;
        }

        // PUT: api/PlayerStates/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerStateData(Guid id, bool? isInGame, string? mapName, Guid? serverUuid, List<Guid>? newFriends)
        {
            PlayerStateData? playerStateData = await _context.player_state.FindAsync(id);

            if (playerStateData == null)
            {
                return NotFound();
            }

            if (id != playerStateData.user_uuid)
            {
                return BadRequest();
            }

            _context.Entry(playerStateData).State = EntityState.Modified;
            var previousServerId = playerStateData.server_uuid;
            if (isInGame != null)
            {
                playerStateData.is_in_game = isInGame.Value;
            }
            if (mapName != null)
            {
                playerStateData.map_name = mapName;
            }
            if (serverUuid != null)
            {
                playerStateData.server_uuid = serverUuid.Value;
                await ComputeServerMMR(playerStateData.server_uuid.Value);
            }
            if (newFriends != null)
            {
                if (playerStateData.friends == null)
                {
                    playerStateData.friends = new List<Guid>();
                }
                foreach (var newFriend in newFriends)
                {
                    playerStateData.friends.Add(newFriend);
                }
            }

            if (previousServerId != null)
            {
                await ComputeServerMMR(previousServerId.Value);
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerStateDataExists(id))
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

        private async Task<int> ComputeServerMMR(Guid serverUid)
        {
            var server = await _context.servers.FindAsync(serverUid);
            var playerStates = await _context.player_state.ToListAsync();

            if (server == null || playerStates == null)
            {
                return 0;
            }

            server.avg_mmr = 0.0f;
            int nbPlayers = 0;
            foreach (var playerState in playerStates)
            {
                var playerStat = await _context.player_stats.FindAsync(playerState.user_uuid);
                if (playerStat == null)
                {
                    continue;
                }
                var playerRank = await _context.ranks.FindAsync(playerStat.rank_uuid);
                if (playerRank == null)
                {
                    continue;
                }

                if (playerState.server_uuid != null && playerState.server_uuid == serverUid)
                {
                    nbPlayers++;
                    server.avg_mmr += playerRank.mmr_value;
                }
            }

            if (nbPlayers > 0)
            {
                server.avg_mmr /= nbPlayers;
            }

            return 1;
        }

        // POST: api/PlayerStates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerStateData>> PostPlayerStateData(PlayerStateData playerStateData)
        {
            _context.player_state.Add(playerStateData);
            if (playerStateData.server_uuid != null)
            {
                await ComputeServerMMR(playerStateData.server_uuid.Value);
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerStateData", new { id = playerStateData.user_uuid }, playerStateData);
        }

        // DELETE: api/PlayerStates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerStateData(Guid id)
        {
            var playerStateData = await _context.player_state.FindAsync(id);
            if (playerStateData == null)
            {
                return NotFound();
            }

            var serverId = playerStateData.server_uuid;
            _context.player_state.Remove(playerStateData);
            await _context.SaveChangesAsync();

            if (serverId != null)
            {
                await ComputeServerMMR(serverId.Value);
                await _context.SaveChangesAsync();
            }

            return NoContent();
        }

        private bool PlayerStateDataExists(Guid id)
        {
            return _context.player_state.Any(e => e.user_uuid == id);
        }
    }
}
