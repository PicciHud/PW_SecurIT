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
    public class HouseController : ControllerBase
    {
        private readonly HouseContext _context;

        public HouseController(HouseContext context)
        {
            _context = context;
        }

        // GET: api/House
        [HttpGet]
        public async Task<ActionResult<IEnumerable<House>>> GetHouse()
        {
          if (_context.House == null)
          {
              return NotFound();
          }
            return await _context.House.ToListAsync();
        }

        // GET: api/House/5
        [HttpGet("{id}")]
        public async Task<ActionResult<House>> GetHouse(int id)
        {
          if (_context.House == null)
          {
              return NotFound();
          }
            var house = await _context.House.FindAsync(id);

            if (house == null)
            {
                return NotFound();
            }

            return house;
        }

        // PUT: api/House/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHouse(int id, House house)
        {
            if (id != house.Id)
            {
                return BadRequest();
            }

            _context.Entry(house).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(id))
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

        // POST: api/House
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<House>> PostHouse(House house)
        {
          if (_context.House == null)
          {
              return Problem("Entity set 'HouseContext.House'  is null.");
          }
            _context.House.Add(house);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetHouse", new { id = house.Id }, house);
        }

        // DELETE: api/House/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHouse(int id)
        {
            if (_context.House == null)
            {
                return NotFound();
            }
            var house = await _context.House.FindAsync(id);
            if (house == null)
            {
                return NotFound();
            }

            _context.House.Remove(house);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool HouseExists(int id)
        {
            return (_context.House?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
