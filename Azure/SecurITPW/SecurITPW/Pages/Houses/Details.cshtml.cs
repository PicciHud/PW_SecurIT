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
    public class DetailsModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public DetailsModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

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
    }
}
