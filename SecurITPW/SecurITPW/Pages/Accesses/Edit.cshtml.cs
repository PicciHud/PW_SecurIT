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

namespace SecurITPW.Pages.Accesses
{
    public class EditModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public EditModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Access Access { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Access == null)
            {
                return NotFound();
            }

            var access =  await _context.Access.FirstOrDefaultAsync(m => m.Id == id);
            if (access == null)
            {
                return NotFound();
            }
            Access = access;
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

            _context.Attach(Access).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AccessExists(Access.Id))
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

        private bool AccessExists(int id)
        {
          return (_context.Access?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
