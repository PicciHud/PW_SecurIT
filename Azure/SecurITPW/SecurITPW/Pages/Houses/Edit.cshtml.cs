using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.Houses
{
    public class EditModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public EditModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        [BindProperty]
        public House House { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.House == null)
            {
                return NotFound();
            }

            var house =  await _context.House.FirstOrDefaultAsync(m => m.Id == id);
            if (house == null)
            {
                return NotFound();
            }
            House = house;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(House).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HouseExists(House.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool HouseExists(int id)
        {
          return (_context.House?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
