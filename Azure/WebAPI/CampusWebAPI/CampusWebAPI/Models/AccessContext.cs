using Microsoft.EntityFrameworkCore;

namespace CampusWebAPI.Models
{
    public class AccessContext : DbContext
    {
        public AccessContext(DbContextOptions<AccessContext> options)
        : base(options)
        {
        }
        public DbSet<Access> Access { get; set; } = null!;
    }
}