using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;


namespace SecurITPW.Pages.Codes
{
    [Authorize] // Aggiungiamo l'attributo Authorize per richiedere l'autenticazione

    public class NomePaginaModel : PageModel
    {
        [BindProperty]
        public string? Code { get; set; }

        public string? SecondCode { get; set; }

        public void OnGet()
        {
            // Codice esempio per popolare il secondo codice (simulazione di una chiamata)
            SecondCode = "code";
        }

        public IActionResult OnPost()
        {
            // Esegui le operazioni necessarie per il codice inviato
            // ...

            // Imposta il messaggio di conferma
            TempData["Message"] = "Codice inviato!";

            // Redirect alla stessa pagina per evitare invii multipli
            return RedirectToPage();
        }
    }
}
