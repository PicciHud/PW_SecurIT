namespace SecurITPW.Models
{
    public class House
    {
        public required int Id { get; set; }                 // Primary key
        public required int IdHouse { get; set; }
        public required int IdRoom { get; set; }
        // public ICollection<Access> Accesses { get; set; }

        // La proprietà Accesses è definita come ICollection<Enrollment> perché potrebbero
        // essere presenti più entità Access correlate
    }
}
