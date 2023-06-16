namespace SecurITPW.Models
{
    public class House
    {
        public int Id { get; set; }                 // Primary key
        public int IdHouse { get; set; }
        public int IdRoom { get; set; }
        // public ICollection<Access> Accesses { get; set; }

        // La proprietà Accesses è definita come ICollection<Enrollment> perché potrebbero
        // essere presenti più entità Access correlate
    }
}
