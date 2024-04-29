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

        // POST: api/PlayerStates
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerStateData>> PostPlayerStateData(PlayerStateData playerStateData)
        {
            _context.player_state.Add(playerStateData);
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

            _context.player_state.Remove(playerStateData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerStateDataExists(Guid id)
        {
            return _context.player_state.Any(e => e.user_uuid == id);
        }
    }
}
