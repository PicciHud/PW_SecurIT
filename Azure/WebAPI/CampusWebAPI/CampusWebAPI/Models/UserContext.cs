using Microsoft.EntityFrameworkCore;

namespace CampusWebAPI.Models
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
        : base(options)
        {
        }
        public DbSet<User> User { get; set; } = null!;
    }
}
