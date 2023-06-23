using Microsoft.EntityFrameworkCore;

namespace CampusWebAPI.Models
{
    public class HouseContext : DbContext
    {
        public HouseContext(DbContextOptions<HouseContext> options)
        : base(options)
        {
        }
        public DbSet<House> House { get; set; } = null!;
    }
}
