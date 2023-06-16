using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.Houses
{
    public class DeleteModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public DeleteModel(SecurITPW.Data.SecurITPWContext context)
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

            var house = await _context.House.FirstOrDefaultAsync(m => m.Id == id);

            if (house == null)
            {
                return NotFound();
            }
            else 
            {
                House = house;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.House == null)
            {
                return NotFound();
            }
            var house = await _context.House.FindAsync(id);

            if (house != null)
            {
                House = house;
                _context.House.Remove(House);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
