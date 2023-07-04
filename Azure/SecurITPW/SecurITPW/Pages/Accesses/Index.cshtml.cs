using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Models;

namespace SecurITPW.Pages.Accesses
{
    //[Authorize(Roles = "Administrator")] //Solo gli utenti del gruppo Admin posso accedere alla pagina
    //[Authorize(Roles = "Admin")] //Solo gli utenti del gruppo Admin posso accedere alla pagina

    public class IndexModel : PageModel
    {
        private readonly SecurITPW.Data.SecurITPWContext _context;

        public IndexModel(SecurITPW.Data.SecurITPWContext context)
        {
            _context = context;
        }

        //dichiarato per il bottone
        public List<Access> Access { get; set; } = default!;

        public async Task OnGetAsync()
        {
            //dichiarato per il bottone
            //Access = await _context.Access.ToListAsync();

            if (_context.Access != null)
            {
                // Ordina la lista degli accessi in modo decrescente in base all'orario
                Access = await _context.Access.OrderByDescending(a => a.Time).ToListAsync();
            }
        }

        public async Task<IActionResult> OnPostDeleteAll()
        {
            // Crea un'istanza di HttpClient
            var httpClient = new HttpClient();

            // Effettua la chiamata all'API
            var response = await httpClient.GetAsync("https://localhost:7061/api/Access");

            if (response.IsSuccessStatusCode)
            {
                // Rimuovi eventuali dati nella cache o esegui altre operazioni necessarie dopo la cancellazione della lista
                // ...

                return RedirectToPage("Index");
            }
            else
            {
                // Gestisci l'errore in modo appropriato
                return StatusCode((int)response.StatusCode);
            }
        }
    }
}



