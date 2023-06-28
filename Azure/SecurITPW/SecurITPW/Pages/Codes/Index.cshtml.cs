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
using System.Security.Permissions;
using System.Text;

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
        public string codeForWeb { get; set; }
        public string codeForPIC { get; set; }
        public string NewCode { get; set; }

        public void OnGet()
        {
            // Viene inizialmente popolato qui per poter vedere la casella di testo sul FE
            NewCode = " ";
        }

        public IActionResult OnPost()
        {
            var codicePic = "";
            var codiceCloud = "";
            // Esegui le operazioni necessarie per il codice inviato
            if (!codeForWeb.IsNullOrEmpty() && codeForWeb.Length == 5)
            {

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                TempData["Message"] = "Codice inviato!";

                // Prendi codice da DB
                TakeCodeToConfront(codicePic);
                // Verifica che il codice sia uguale a quello inserito
                var equal = ConfrontCodes(codicePic, codeForWeb);
                if (equal == false)
                {
                    TempData["Message"] = "CODICE ERRATO";
                }

                // Se uguale 
                if (equal == true)
                {
                    //crea nuovo codice
                    codeForPIC = CreateNewCode();

                    // Invia il nuovo codice a DB

                    // Prendi altro codice da DB(quello che arriva dal PIC, che ha inserito l'utente)


                    // Verifica che il codice preso da DB sia uguale a quello nuovo inviato appena prima


                    // Se uguale ritorna valore che apre la porta
                }

                return Page();
            }

            if (codeForWeb.IsNullOrEmpty() || codeForWeb.Length < 5)
            {
                // Imposta il messaggio di avviso che il codice non è stato inviato
                TempData["Message"] = "Codice errato: troppo corto";
            }

            // Redirect alla stessa pagina per evitare invii multipli
            return RedirectToPage();
        }

        public async void TakeCodeToConfront(string codicePic)
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

                // DA SISTEMARE IL CONTROLLO NEL DB
                foreach (var code in codes)
                {
                    // Visualizza i dettagli del prodotto
                    codicePic = code.CodePic;
                }
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
            }
        }

        public bool ConfrontCodes(string codicePic, string codeForWeb)
        {
            return codicePic.Equals(codeForWeb);
        }

        public string CreateNewCode()
        {

            string Characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            StringBuilder stringBuilder = new StringBuilder(5);

            for (int i = 0; i < 5; i++)
            {
                 int index = random.Next(Characters.Length);
                 char randomChar = Characters[index];
                 stringBuilder.Append(randomChar);
            }
            
            return stringBuilder.ToString();
        }
    }
}





//CODICE PER USARE UN'API
//public async void callForAPI()
//{
//    // Crea un'istanza di HttpClient
//    var httpClient = new HttpClient();

//    // Effettua la chiamata all'API
//    var response = await httpClient.GetAsync("https://localhost:7061/api/Access");

//    if (response.IsSuccessStatusCode)
//    {
//        // Deserializza la risposta in una lista di oggetti Product
//        var codes = await response.Content.ReadFromJsonAsync<List<Access>>();

//        // Utilizza i dati ottenuti dall'API come desiderato
//        foreach (var code in codes)
//        {
//            // Visualizza i dettagli del prodotto
//            var codicePic = code.CodePic;
//            var codiceCloud =code.CodeCloud;
//        }
//    }
//    else
//    {
//        // Gestisci eventuali errori
//        TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
//    }
//}
