using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.Accesses
{
    public class DetailsModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public DetailsModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

      public Access Access { get; set; } = default!; 

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Access == null)
            {
                return NotFound();
            }

            var access = await _context.Access.FirstOrDefaultAsync(m => m.Id == id);
            if (access == null)
            {
                return NotFound();
            }
            else 
            {
                Access = access;
            }
            return Page();
        }
    }
}
