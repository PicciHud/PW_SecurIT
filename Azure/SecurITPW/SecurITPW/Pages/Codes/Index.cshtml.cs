using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

//PER USARE LE API
// Includi i namespace necessari
using System.Text;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.Azure.Devices; //da installare con Nuget se non la trova
using NuGet.Protocol;

//Per ServiceBus
using Microsoft.Azure.ServiceBus;

namespace SecurITPW.Pages.Codes
{
    [Authorize] // Aggiungiamo l'attributo Authorize per richiedere l'autenticazione

    public class IndexModel : PageModel
    {
        [BindProperty]
        public string codeForWeb { get; set; }
        public string codeForPIC { get; set; }
        public string NewCode { get; set; }
        public bool UnlockDoor { get; set; }
        //variabile per dare errore nel caso in cui si digita sbagliato il codice nel pic
        public bool equal2 { get; set; }
        public string messaggio { get; set; }


        private readonly IConfiguration _configuration;

        // Per ServiceBus
        //"Your_ServiceBus_Connection_String";
        private const string ServiceBusConnectionString = "Endpoint=sb://securit.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=RFtJ267H1qEUxXv73KqDILGAp23xMocRs+ASbCF4ajY=";
        //"Your_Queue_Name";
        private const string QueueName = "SecurIT";

        // Per IotHub
        private ServiceClient _serviceClient;
        private readonly string _iotHubConn = "HostName=SecurIT.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=VRySmDOXf1a6L7G6e3C73FlugiorWXEVbsboH2G1AlA="; // DA METTERE CHE CORRISPONDE ALLA STRINGA DI CONNESSIONE DELL' IotHub??

        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            string connectionString = _configuration.GetConnectionString("SecurITPWContext"); //DefaultConnection (non ho capito quale delle due serve per connettersi al DB)

            if (connectionString == null)
            {
                throw new Exception("Impossibile connettersi al DataBase");
            }
        }

        // creato per fare il messaggio per l'IotHub
        public class Access
        {
            public int Id { get; set; }
            public string? CodePic { get; set; }
            public string? CodeCloud { get; set; }
            public int IdPic { get; set; }
            public int? IdUser { get; set; }
            public string? Name { get; set; }
            public string? SurName { get; set; }
            public int IdHouse { get; set; }
            public int IdRoom { get; set; }
            public DateTime Time { get; set; }
        }

        Access access = new Access();

        public async Task<IActionResult> OnGet()
        {
            // Viene inizialmente popolato qui per poter vedere la casella di testo sul FE
            NewCode = "";

            // Per ServiceBus
            //ricevi json dal service bus per prendere IdPic, IdRoom, IdHouse e Time //me li inizializza??
            access = await ReceiveAndDeserializeJson();

            return Page();
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
                var codicePic = await TakeCodeFromDBForWeb(access);
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
                    var codiceCloud = codeForPIC;
                                      //await TakeCodeFromDBForPIC(codicePic);

                    // Verifica che il codice preso da DB sia uguale a quello nuovo creato appena prima
                    var equal2 = ConfrontCodes(codiceCloud, codeForPIC);

                    // Se diverso avvisa che è stato inserito errato
                    if (equal2 == false)
                    {
                        messaggio = "CODICE INSERITO NEL PIC ERRATO";
                    }
                    // DA SISTEMAREEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE
                    // SALVA ALL'INTERNO DEL JSON ANCHE LO USERNAME CON LA MAIL E LASCIA IL SURNAME VUOTO
                    // Se uguale ritorna valore che apre la porta
                    if (equal2 == true)
                    {
                        access = await inizializedAccess(access, codiceCloud);

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

        public async Task<string> TakeCodeFromDBForWeb(Access access)
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
                // IL JSON RITORNA IdPic e IdRoom e IdHouse e Time, PRENDERE L'ELEMENTO CON QUELLI
                // Ordina la lista in base alla colonna IdPic e IdRoom e IdHouse e Time in ordine decrescente
                codes = codes.OrderByDescending(d => d.Time)
                             .ThenByDescending(d => d.IdPic)
                             .ThenByDescending(d => d.IdRoom)
                             .ThenByDescending(d => d.IdHouse)
                             .ToList();

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

        public bool ConfrontCodes(string codicePic, string codeForWeb)
        {
            string codeWithout = codicePic.Replace("\r", "");
            return codeWithout.Equals(codeForWeb);
        }

        public string CreateNewCode()
        {

            string caratteri = "0123456789";
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

        public async Task<Access> inizializedAccess(Access access, string codiceCloud)
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
                // IL JSON RITORNA IdPic e IdRoom e IdHouse e Time, PRENDERE L'ELEMENTO CON QUELLI
                // Ordina la lista in base alla colonna IdPic e IdRoom e IdHouse e Time in ordine decrescente
                messageToIotHub = messageToIotHub.OrderByDescending(d => d.Time)
                             .ThenByDescending(d => d.IdPic)
                             .ThenByDescending(d => d.IdRoom)
                             .ThenByDescending(d => d.IdHouse)
                             .ToList();

                //Prendi il primo elemento(l'ultimo in base all'ordinamento)
                var latestAccess = messageToIotHub.FirstOrDefault();

                string codePicWithout = latestAccess.CodePic.Replace("\r", "");

                // valorizza access
                access.Id = latestAccess.Id;
                access.CodePic = codePicWithout;
                access.CodeCloud = codiceCloud;
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

        // Per ServiceBus
        private async Task<Access> ReceiveAndDeserializeJson()
        {
            QueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName);

            // Registrazione del gestore del messaggio
            queueClient.RegisterMessageHandler(ProcessMessagesAsync, new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 1 });

            // Attesa di 5 secondi per la ricezione dei messaggi
            await Task.Delay(TimeSpan.FromSeconds(5));

            // Chiusura del client e del gestore del messaggio
            await queueClient.CloseAsync();

            return _receivedAccess;
        }

        private Access _receivedAccess;

        private async Task ProcessMessagesAsync(Microsoft.Azure.ServiceBus.Message message, CancellationToken token)
        {
            string json = Encoding.UTF8.GetString(message.Body);

            _receivedAccess = JsonConvert.DeserializeObject<Access>(json);

            // Completare il messaggio per rimuoverlo dalla coda
            QueueClient queueClient = new QueueClient(ServiceBusConnectionString, QueueName);
            await queueClient.CompleteAsync(message.SystemProperties.LockToken);
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            Console.WriteLine($"Exception occurred: {exceptionReceivedEventArgs.Exception}");
            return Task.CompletedTask;
        }
    }






    //// Alla fine è inutile, la tengo per sicurezza
    //public async Task<string> TakeCodeFromDBForPIC(string codicePic)
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
    //        // DA SISTEMARE IL CONTROLLO NEL DB??
    //        foreach (var code in codes)
    //        {
    //            if (codicePic == code.CodePic)
    //            {
    //                if (code.CodeCloud == null)
    //                {
    //                    TempData["Message"] = "ERRORE: \\nCodice inesistente nel DB, impossibile aprire la porta";
    //                    return null;
    //                }
    //                return code.CodeCloud;
    //            }
    //        }
    //        return null;
    //    }
    //    else
    //    {
    //        // Gestisci eventuali errori
    //        TempData["Message"] = "Si è verificato un errore durante la chiamata all'API";
    //        return null;
    //    }
    //}
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
