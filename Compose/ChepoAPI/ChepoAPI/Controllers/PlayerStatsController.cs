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
    public class PlayerStatsController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public PlayerStatsController(PostgreDbContext context)
        {
            _context = context;
        }

        // GET: api/PlayerStatsDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PlayerStatsData>>> GetPlayerStats()
        {
            return await _context.player_stats.ToListAsync();
        }

        // GET: api/PlayerStatsDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PlayerStatsData>> GetPlayerStatsData(Guid id)
        {
            var playerStatsData = await _context.player_stats.FindAsync(id);

            if (playerStatsData == null)
            {
                return NotFound();
            }

            return playerStatsData;
        }

        // PUT: api/PlayerStatsDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPlayerStatsData(Guid id, Guid? newRankId, int? newKillCount, int? newDeathCount)
        {
            PlayerStatsData? playerStatsData = await _context.player_stats.FindAsync(id);

            if (playerStatsData == null)
            {
                return NotFound();
            }

            if (id != playerStatsData.user_uuid)
            {
                return BadRequest();
            }

            _context.Entry(playerStatsData).State = EntityState.Modified;
            if (newRankId != null)
            {
                playerStatsData.rank_uuid = newRankId.Value;
            }
            if (newKillCount != null)
            {
                playerStatsData.kill = newKillCount.Value;
            }
            if (newDeathCount != null)
            {
                playerStatsData.death = newDeathCount.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlayerStatsDataExists(id))
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

        // POST: api/PlayerStatsDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PlayerStatsData>> PostPlayerStatsData(PlayerStatsData playerStatsData)
        {
            _context.player_stats.Add(playerStatsData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayerStatsData", new { id = playerStatsData.user_uuid }, playerStatsData);
        }

        // DELETE: api/PlayerStatsDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePlayerStatsData(Guid id)
        {
            var playerStatsData = await _context.player_stats.FindAsync(id);
            if (playerStatsData == null)
            {
                return NotFound();
            }

            _context.player_stats.Remove(playerStatsData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PlayerStatsDataExists(Guid id)
        {
            return _context.player_stats.Any(e => e.user_uuid == id);
        }
    }
}
