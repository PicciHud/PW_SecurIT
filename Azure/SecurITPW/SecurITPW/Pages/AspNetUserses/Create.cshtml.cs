using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.AspNetUserses
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
        public AspNetUsers AspNetUsers { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.AspNetUsers == null || AspNetUsers == null)
            {
                return Page();
            }

            _context.AspNetUsers.Add(AspNetUsers);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
