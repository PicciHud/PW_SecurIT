using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Models;

namespace SecurITPW.Pages.AspNetUserses
{
    [Authorize] // Aggiungiamo l'attributo Authorize per richiedere l'autenticazione

    public class IndexModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public IndexModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        public IList<AspNetUsers> AspNetUsers { get;set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.AspNetUsers != null)
            {
                AspNetUsers = await _context.AspNetUsers.ToListAsync();
            }
        }
    }
}
