using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SecurITPW.Models
{
    public class Access
    {
        public int Id { get; set; }                 // Primary key
        public int IdPic { get; set; }
        public int IdUser { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public int IdHouse { get; set; }
        public int IdRoom { get; set; }
        public DateTime Time { get; set; }
    }
}


// Per la creazione del DB
// Add-Migration InitialCreate
// Update - Database