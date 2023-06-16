using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.Accesses
{
    public class CreateModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public CreateModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Access Access { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Access == null || Access == null)
            {
                return Page();
            }

            _context.Access.Add(Access);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
