using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

//PER USARE LE API
// Includi i namespace necessari
using SecurITPW.Models;
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
        public bool UnlockDoor { get; set; }
        //variabile per dare errore nel caso in cui si digita sbagliato il codice nel pic
        public bool equal2 { get; set; }

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
                TakeCodeFromDBForWeb(codicePic);
                // Verifica che il codice sia uguale a quello inserito
                var equal1 = ConfrontCodes(codicePic, codeForWeb);
                if (equal1 == false)
                {
                    TempData["Message"] = "CODICE ERRATO";
                }

                // Se uguale 
                if (equal1 == true)
                {
                    //crea nuovo codice
                    codeForPIC = CreateNewCode();

                    // Prendi il codiche che il PIC manda al DB
                    TakeCodeFromDBForPIC(codiceCloud, codicePic);

                    // Verifica che il codice preso da DB sia uguale a quello nuovo creato appena prima
                    equal2 = ConfrontCodes(codiceCloud, codeForPIC);

                    if (equal2 == false)
                    {
                        TempData["Message"] = "CODICE INSERITO NEL PIC ERRATO";
                    }
                    // Se uguale ritorna valore che apre la porta
                    if (equal2 == true)
                    {
                        UnlockDoor = true;
                    }
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

        public async void TakeCodeFromDBForWeb(string codicePIC)
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
                // DA SISTEMARE IL CONTROLLO NEL DB, prendere l'ultimo codice
                foreach (var code in codes)
                {
                    // Visualizza i dettagli del prodotto
                    codicePIC = code.CodePic;
                }
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
            }
        }

        public async void TakeCodeFromDBForPIC(string codiceCloud, string codicePic)
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
                    if (codicePic == code.CodePic)
                    {
                        codiceCloud = code.CodeCloud;
                    }
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

            string caratteri = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();

            StringBuilder codice = new StringBuilder(5);

            for (int i = 0; i < 5; i++)
            {
                 int indice = random.Next(caratteri.Length);
                 char carattere = caratteri[indice];
                codice.Append(carattere);
            }
            
            return codice.ToString();
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
