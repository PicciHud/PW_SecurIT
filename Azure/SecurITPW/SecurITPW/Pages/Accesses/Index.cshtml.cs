using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Data;
using SecurITPW.Models;

namespace SecurITPW.Pages.Accesses
{
    [Authorize(Roles = "Admin")] //Solo gli utenti del gruppo Admin posso accedere alla pagina
    public class IndexModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public IndexModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        public IList<Access> Access { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Access != null)
            {
                Access = await _context.Access.ToListAsync();
            }
        }
    }
}
