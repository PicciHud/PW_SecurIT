using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.AspNetUserses
{
    public class DetailsModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public DetailsModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

      public AspNetUsers AspNetUsers { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspnetusers = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetusers == null)
            {
                return NotFound();
            }
            else 
            {
                AspNetUsers = aspnetusers;
            }
            return Page();
        }
    }
}
