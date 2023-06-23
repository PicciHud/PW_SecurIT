namespace CampusWebAPI.Models
{
    public class User
    {
        public required int Id { get; set; }                 // Primary key
        public required int IdUser { get; set; }             
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }

        // public ICollection<Access> Accesses { get; set; }

        // La proprietà Accesses è definita come ICollection<Enrollment> perché potrebbero
        // essere presenti più entità Access correlate
    }
}
