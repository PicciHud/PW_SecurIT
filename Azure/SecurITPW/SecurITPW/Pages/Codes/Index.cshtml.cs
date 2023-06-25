using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient; //bisogna anche installare System.Data.SqlClient dalla gestione pacchetti nuget
using System.Dynamic;

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

                // Salva il codice nel database
                SaveCodeToDatabase(Code);

                // Ottieni il nuovo codice dal database
                NewCode = GetNewCodeFromDatabase();

                // Imposta il messaggio di conferma
                TempData["Message"] = "Codice inviato!";

                return Page();
            }
            if(Code.IsNullOrEmpty())
            {
                // Imposta il messaggio di avviso che il codice non è stato inviato
                TempData["Message"] = "CODICE MANCANTE";
            }

            // Redirect alla stessa pagina per evitare invii multipli
            return RedirectToPage();
        }

        public IActionResult OnPostCheckCode(string codeToCheck)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Controlla se il codice inserito corrisponde a un altro codice nel database
            bool isCodeMatching = CheckCodeWithDatabase(codeToCheck);

            // Esegui l'azione appropriata in base al risultato del controllo
            if (isCodeMatching)
            {
                // Il codice inserito corrisponde a un altro codice nel database
                TempData["Message"] = "CODICE ERRATO";
            }
            else
            {
                // Il codice inserito non corrisponde a nessun altro codice nel database
                TempData["Message"] = "CODICE INESISTENTE";
            }

            return Page();
        }

        private void SaveCodeToDatabase(string code)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //cambiare nome tabella "YourTable" con il nome della tabella a db che contiene il codice
                SqlCommand command = new SqlCommand("INSERT INTO Codes (codeFromPIC) VALUES (@Code)", connection);
                command.Parameters.AddWithValue("@Code", code);
                command.ExecuteNonQuery();
            }
        }

        private string GetNewCodeFromDatabase()
        {
            string newCode = string.Empty;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //cambiare nome tabella "YourTable" con il nome della tabella a db che contiene il codice
                SqlCommand command = new SqlCommand("SELECT TOP 1 codeFromWeb FROM Codes ORDER BY Id DESC", connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    newCode = reader.GetString(0);
                }
            }
            return newCode;
        }

        private bool CheckCodeWithDatabase(string codeToCheck)
        {
            bool isMatching = false;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                //cambiare nome tabella "YourTable" con il nome della tabella a db che contiene il codice
                SqlCommand command = new SqlCommand("SELECT COUNT(*) FROM YourTable WHERE Code = @Code", connection);
                command.Parameters.AddWithValue("@Code", codeToCheck);
                int count = (int)command.ExecuteScalar();
                if (count > 0)
                {
                    isMatching = true;
                }
            }
            return isMatching;
        }

    }
}
