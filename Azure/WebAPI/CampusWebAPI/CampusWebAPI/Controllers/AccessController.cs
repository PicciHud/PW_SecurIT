using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CampusWebAPI.Models;

namespace CampusWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly AccessContext _context;

        public AccessController(AccessContext context)
        {
            _context = context;
        }

        // GET: api/Access
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Access>>> GetAccess()
        {
          if (_context.Access == null)
          {
              return NotFound();
          }
            return await _context.Access.ToListAsync();
        }

        // GET: api/Access/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Access>> GetAccess(int id)
        {
          if (_context.Access == null)
          {
              return NotFound();
          }
            var access = await _context.Access.FindAsync(id);

            if (access == null)
            {
                return NotFound();
            }

            return access;
        }

        // PUT: api/Access/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAccess(int id, Access access)
        {
            if (id != access.Id)
            {
                return BadRequest();
            }

            _context.Entry(access).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccessExists(id))
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

        // POST: api/Access
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Access>> PostAccess(Access access)
        {
          if (_context.Access == null)
          {
              return Problem("Entity set 'AccessContext.Access'  is null.");
          }
            _context.Access.Add(access);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAccess", new { id = access.Id }, access);
        }

        // DELETE: api/Access/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccess(int id)
        {
            if (_context.Access == null)
            {
                return NotFound();
            }
            var access = await _context.Access.FindAsync(id);
            if (access == null)
            {
                return NotFound();
            }

            _context.Access.Remove(access);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AccessExists(int id)
        {
            return (_context.Access?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
