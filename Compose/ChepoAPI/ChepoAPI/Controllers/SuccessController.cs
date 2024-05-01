using ChepoAPI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using ChepoAPI.Interfaces;

namespace ChepoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class SuccessController : ControllerBase
    {
        private readonly PostgreDbContext _context;
        private readonly ICacheService _cacheService;

        public SuccessController(PostgreDbContext context, ICacheService cacheService)
        {
            _context = context;
            _cacheService = cacheService;
        }

        // GET: api/Success
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SuccessData>>> Getsuccess()
        {
            var cacheData = _cacheService.GetData<List<SuccessData>>("successList");
            if (cacheData == null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                cacheData = await _context.success.ToListAsync();
                _cacheService.SetData("successList", cacheData, expirationTime);
            }

            return cacheData;
        }

        // GET: api/Success/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SuccessData>> GetSuccessData(Guid id)
        {
            var cacheData = _cacheService.GetData<SuccessData>("successData" + id.ToString());
            if (cacheData == null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                cacheData = await _context.success.FindAsync(id);
                _cacheService.SetData("successData" + id.ToString(), cacheData, expirationTime);
            }

            if (cacheData == null)
            {
                return NotFound();
            }

            return cacheData;
        }

        [HttpPost("grant")]
        public async Task<ActionResult> GrantSuccessToPlayer(Guid userId, Guid successId)
        {
            var userIdString = userId.ToString();
            var user = _cacheService.GetData<UsersData>(userIdString);
            if (user == null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                user = await _context.users.FindAsync(userId);
                _cacheService.SetData(userIdString, user, expirationTime);
            }

            var successIdString = successId.ToString();
            var success = _cacheService.GetData<SuccessData>(successIdString);
            if(success == null)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5.0);
                success = await _context.success.FindAsync(successId);
                _cacheService.SetData(successIdString, success, expirationTime);
            }

            if (user == null || success == null)
            {
                return BadRequest("Utilisateur ou succès introuvable !");
            }

            return Ok("Succès accordé !");
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
