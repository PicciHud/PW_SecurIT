namespace CampusWebAPI.Models
{
    public class Access
    {
        public required int Id { get; set; }                 // Primary key
        public required int IdPic { get; set; }
        public required int IdUser { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public required int IdHouse { get; set; }
        public required int IdRoom { get; set; }
        public DateTime Time { get; set; }
    }
}
