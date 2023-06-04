using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        // GET
        public void OnGet()
        {

        }

        // POST
        public IActionResult OnPost() // Identico a void, reiderizza la pagina
        {
           // return Page(); 
           if (!ModelState.IsValid)
            {
                return Page();            
            }

           // Todo: save ti db

            return RedirectToPage("/Index");
        }
    }
}