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
    public class SuccessController : ControllerBase
    {
        private readonly PostgreDbContext _context;

        public SuccessController(PostgreDbContext context)
        {
            _context = context;
        }

        // GET: api/Success
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuccessData>>> Getsuccess()
        {
            return await _context.success.ToListAsync();
        }

        // GET: api/Success/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SuccessData>> GetSuccessData(Guid id)
        {
            var successData = await _context.success.FindAsync(id);

            if (successData == null)
            {
                return NotFound();
            }

            return successData;
        }

        // PUT: api/Success/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSuccessData(Guid id, string? name, string? description, string? image)
        {
            SuccessData? successData = await _context.success.FindAsync(id);

            if (successData == null)
            {
                return NotFound();
            }

            if (id != successData.uuid)
            {
                return BadRequest();
            }

            _context.Entry(successData).State = EntityState.Modified;
            if (name != null)
            {
                successData.name = name;
            }
            if (description != null)
            {
                successData.description = description;
            }
            if (image != null)
            {
                successData.image = image;
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SuccessDataExists(id))
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

        // POST: api/Success
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SuccessData>> PostSuccessData(string name, string description, string image)
        {
            SuccessData successData = new SuccessData();
            successData.uuid = Guid.NewGuid();
            successData.name = name;
            successData.description = description;
            successData.image = image;
            _context.success.Add(successData);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSuccessData", new { id = successData.uuid }, successData);
        }

        // DELETE: api/Success/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSuccessData(Guid id)
        {
            var successData = await _context.success.FindAsync(id);
            if (successData == null)
            {
                return NotFound();
            }

            _context.success.Remove(successData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SuccessDataExists(Guid id)
        {
            return _context.success.Any(e => e.uuid == id);
        }
    }
}
