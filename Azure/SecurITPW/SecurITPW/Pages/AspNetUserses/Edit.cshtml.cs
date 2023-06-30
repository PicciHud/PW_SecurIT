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

namespace SecurITPW.Pages.AspNetUserses
{
    public class EditModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public EditModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        [BindProperty]
        public AspNetUsers AspNetUsers { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspnetusers =  await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == id);
            if (aspnetusers == null)
            {
                return NotFound();
            }
            AspNetUsers = aspnetusers;
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

            _context.Attach(AspNetUsers).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AspNetUsersExists(AspNetUsers.Id))
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

        private bool AspNetUsersExists(string id)
        {
          return (_context.AspNetUsers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
