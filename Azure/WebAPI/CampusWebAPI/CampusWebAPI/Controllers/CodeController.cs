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
    public class CodeController : ControllerBase
    {
        private readonly CodeContext _context;

        public CodeController(CodeContext context)
        {
            _context = context;
        }

        // GET: api/Code
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Code>>> GetCode()
        {
          if (_context.Code == null)
          {
              return NotFound();
          }
            return await _context.Code.ToListAsync();
        }

        // GET: api/Code/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Code>> GetCode(int id)
        {
          if (_context.Code == null)
          {
              return NotFound();
          }
            var code = await _context.Code.FindAsync(id);

            if (code == null)
            {
                return NotFound();
            }

            return code;
        }

        // PUT: api/Code/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCode(int id, Code code)
        {
            if (id != code.Id)
            {
                return BadRequest();
            }

            _context.Entry(code).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CodeExists(id))
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

        // POST: api/Code
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Code>> PostCode(Code code)
        {
          if (_context.Code == null)
          {
              return Problem("Entity set 'CodeContext.Code'  is null.");
          }
            _context.Code.Add(code);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCode", new { id = code.Id }, code);
        }

        // DELETE: api/Code/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCode(int id)
        {
            if (_context.Code == null)
            {
                return NotFound();
            }
            var code = await _context.Code.FindAsync(id);
            if (code == null)
            {
                return NotFound();
            }

            _context.Code.Remove(code);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CodeExists(int id)
        {
            return (_context.Code?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
