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
    public class IndexModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public IndexModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        public IList<House> House { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.House != null)
            {
                House = await _context.House.ToListAsync();
            }
        }
    }
}
