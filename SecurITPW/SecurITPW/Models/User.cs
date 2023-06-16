namespace SecurITPW.Models
{
    public class User
    {
        public int Id { get; set; }                 // Primary key
        public int IdUser { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }

        // public ICollection<Access> Accesses { get; set; }

        // La proprietà Accesses è definita come ICollection<Enrollment> perché potrebbero
        // essere presenti più entità Access correlate
    }
}
