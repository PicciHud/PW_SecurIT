using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient; //bisogna anche installare System.Data.SqlClient dalla gestione pacchetti nuget
using System.Dynamic;
using System.Net.NetworkInformation;
//PER USARE LE API
// Includi i namespace necessari
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using SecurITPW.Models;


namespace SecurITPW.Pages.Codes
{
    [Authorize] // Aggiungiamo l'attributo Authorize per richiedere l'autenticazione

    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("SecurITPWContext"); //DefaultConnection (non ho capito quale delle due serve per connettersi al DB)

            if (connectionString == null)
            {
                throw new Exception("Impossibile connettersi al DataBase");
            }
        }

        [BindProperty]
        public string Code { get; set; }
        public string NewCode { get; set; }

        public void OnGet()
        {
            // Viene inizialmente popolato qui per poter vedere la casella di testo sul FE
            NewCode = " ";
        }

        public IActionResult OnPost()
        {
            // Esegui le operazioni necessarie per il codice inviato
            if (!Code.IsNullOrEmpty() && Code.Length == 5)
            {

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                TempData["Message"] = "Codice inviato!";

                // Prendi codice da DB

                // Verifica che il codice sia uguale a quello inserito

                // Se uguale crea nuovo codice
                bool isCodeMatching = false;

                if (isCodeMatching == false)
                {
                    // Il codice inserito corrisponde a un altro codice nel database
                    TempData["Message"] = "CODICE ERRATO";
                }

                // Invia il nuovo codice a DB

                // Prendi altro codice da DB(quello che arriva dal PIC, che ha inserito l'utente

                // Verifica che il codice preso da DB sia uguale a quello nuovo inviato appena prima

                // Se uguale ritorna valore che apre la porta

                return Page();
            }
            if (Code.IsNullOrEmpty() || Code.Length < 5)
            {
                // Imposta il messaggio di avviso che il codice non è stato inviato
                TempData["Message"] = "Codice errato: troppo corto";
            }

            // Redirect alla stessa pagina per evitare invii multipli
            return RedirectToPage();
        }

        public async void callForAPI()
        {
            // Crea un'istanza di HttpClient
            var httpClient = new HttpClient();

            // Effettua la chiamata all'API
            var response = await httpClient.GetAsync("https://localhost:7061/api/Access");

            if (response.IsSuccessStatusCode)
            {
                // Deserializza la risposta in una lista di oggetti Product
                var codes = await response.Content.ReadFromJsonAsync<List<Access>>();

                // Utilizza i dati ottenuti dall'API come desiderato
                foreach (var code in codes)
                {
                    // Visualizza i dettagli del prodotto
                    var codicePic = code.CodePic;
                    var codiceCloud =code.CodeCloud;
                }
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
            }
        }
    }
}
