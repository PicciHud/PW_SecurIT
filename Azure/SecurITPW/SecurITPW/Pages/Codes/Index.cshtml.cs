using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

//PER USARE LE API
// Includi i namespace necessari
using SecurITPW.Models;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Azure.Devices; //da installare con Nuget se non la trova
using NuGet.Protocol;
using Microsoft.AspNetCore.DataProtection;

namespace SecurITPW.Pages.Codes
{
    [Authorize] // Aggiungiamo l'attributo Authorize per richiedere l'autenticazione

    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;

        // Per IotHub
        private ServiceClient _serviceClient;
        private readonly string _iotHubConn; // DA METTERE CHE CORRISPONDE ALLA STRINGA DI CONNESSIONE DELL' IotHub??

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
        public string messaggio { get; set; }

        public void OnGet()
        {
            // Viene inizialmente popolato qui per poter vedere la casella di testo sul FE
            NewCode = "";
        }

        // creato per fare il messaggio per l'IotHub
        public class Access
        {
            public int Id { get; set; }
            public string? CodePic { get; set; }
            public string? CodeCloud { get; set; }
            public int IdPic { get; set; }
            public int IdUser { get; set; }
            public string? Name { get; set; }
            public string? SurName { get; set; }
            public int IdHouse { get; set; }
            public int IdRoom { get; set; }
            public DateTime Time { get; set; }
        }

        public async Task<IActionResult> OnPost()
        {
            // Esegui le operazioni necessarie per il codice inviato
            if (!codeForWeb.IsNullOrEmpty() && codeForWeb.Length == 5)
            {

                if (!ModelState.IsValid)
                {
                    return Page();
                }

                TempData["Message"] = "Codice inviato!";

                // Prendi codice da DB
                var codicePic = await TakeCodeFromDBForWeb();
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

                    // Prendi il codice che il PIC manda al DB
                    var codiceCloud = TakeCodeFromDBForPIC(codicePic).ToString();

                    // Verifica che il codice preso da DB sia uguale a quello nuovo creato appena prima
                    equal2 = ConfrontCodes(codiceCloud, codeForPIC);

                    // Se diverso avvisa che è stato inserito errato
                    if (equal2 == false)
                    {
                        messaggio = "CODICE INSERITO NEL PIC ERRATO";
                    }
                    // Se uguale ritorna valore che apre la porta
                    if (equal2 == true)
                    {
                        Access access = new Access();
                        access = await inizializedAccess(access);

                        access.ToJson();
                        
                        // Per IotHub
                        var deviceMessage = new Microsoft.Azure.Devices.Message(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(access)));
                        _serviceClient = ServiceClient.CreateFromConnectionString(_iotHubConn);
                        await _serviceClient.SendAsync("SecurIT-Device", deviceMessage); //deviceId è il nome del device dell'IotHub?? Se si è "SecurIT-Device", ma come gli dico che è quel tipo di device??
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

        public async Task<string> TakeCodeFromDBForWeb()
        {
            // Crea un'istanza di HttpClient
            var httpClient = new HttpClient();

            // Effettua la chiamata all'API
            var response = await httpClient.GetAsync("https://localhost:7061/api/Access");

            if (response.IsSuccessStatusCode)
            {
                // Deserializza la risposta in una lista di oggetti
                var codes = await response.Content.ReadFromJsonAsync<List<Access>>();

                // UTILIZZA I DATI OTTENUTI DALL'API COME DESIDERATO

                // Ordina la lista in base alla colonna "Time" in ordine decrescente
                codes = codes.OrderByDescending(d => d.Time).ToList();

                // Prendi il primo elemento (l'ultimo in base all'ordinamento)
                return codes.FirstOrDefault().CodePic;

                //codicePIC = codes.Count().ToString();
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
                return null;
            }
        }

        public async Task<string> TakeCodeFromDBForPIC(string codicePic)
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
                // DA SISTEMARE IL CONTROLLO NEL DB??
                foreach (var code in codes)
                {
                    if (codicePic == code.CodePic)
                    {
                        if(code.CodeCloud == null)
                        {
                            TempData["Message"] = "ERRORE: \\nCodice inesistente nel DB, impossibile aprire la porta";
                            return null;
                        }
                        return code.CodeCloud;
                    }
                }
                return null;
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
                return null;
            }
        }

        public bool ConfrontCodes(string codicePic, string codeForWeb)
        {
            string codeWithout = codicePic.Replace("\r", "");
            return codeWithout.Equals(codeForWeb);
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

        public async Task<Access> inizializedAccess(Access access)
        {
            // Crea un'istanza di HttpClient
            var httpClient = new HttpClient();

            // Effettua la chiamata all'API
            var response = await httpClient.GetAsync("https://localhost:7061/api/Access");

            if (response.IsSuccessStatusCode)
            {
                // Deserializza la risposta in una lista di oggetti
                var messageToIotHub = await response.Content.ReadFromJsonAsync<List<Access>>();

                // UTILIZZA I DATI OTTENUTI DALL'API COME DESIDERATO

                // Ordina la lista in base alla colonna "Time" in ordine decrescente
                messageToIotHub = messageToIotHub.OrderByDescending(d => d.Time).ToList();

                //Prendi il primo elemento(l'ultimo in base all'ordinamento)
                messageToIotHub.FirstOrDefault();

                var latestAccess = messageToIotHub.FirstOrDefault();

                // valorizza access
                access.Id = latestAccess.Id;
                access.CodePic = latestAccess.CodePic;
                access.CodeCloud = latestAccess.CodeCloud;
                access.IdPic = latestAccess.IdPic;
                access.IdUser = latestAccess.IdUser;
                access.Name = latestAccess.Name;
                access.SurName = latestAccess.SurName;
                access.IdHouse = latestAccess.IdHouse;
                access.IdRoom = latestAccess.IdRoom;
                access.Time = latestAccess.Time;

                return access;
            }
            else
            {
                // Gestisci eventuali errori
                TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
                return null;
            }
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
