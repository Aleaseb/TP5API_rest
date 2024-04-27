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
    [Route("[controller]")]
    [ApiController]
    public class RanksController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public RanksController(PostgreDbContext context)
        {
            _context = context;
        }

        // GET: api/Rank
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RankData>>> GetRanks()
        {
            return await _context.rank.ToListAsync();
        }

        // GET: api/Rank/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RankData>> GetRankData(Guid id)
        {
            var rankData = await _context.rank.FindAsync(id);

            if (rankData == null)
            {
                return NotFound();
            }

            return rankData;
        }

        // PUT: api/Rank/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRankData(Guid id, string newRankName)
        {
            var rankData = await _context.rank.FindAsync(id);
            if (rankData == null)
            {
                return NotFound();
            }

            if (id != rankData.uuid)
            {
                return BadRequest();
            }

            _context.Entry(rankData).State = EntityState.Modified;
            rankData.name = newRankName;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RankDataExists(id))
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

        // POST: api/Rank
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<RankData>> PostRankData(string rankName)
        {
            RankData rankData = new RankData();
            rankData.uuid = Guid.NewGuid();
            rankData.name = rankName;
            _context.rank.Add(rankData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRankData", new { id = rankData.uuid }, rankData);
        }

        // DELETE: api/Rank/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRankData(Guid id)
        {
            var rankData = await _context.rank.FindAsync(id);
            if (rankData == null)
            {
                return NotFound();
            }

            _context.rank.Remove(rankData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RankDataExists(Guid id)
        {
            return _context.rank.Any(e => e.uuid == id);
        }
    }
}
