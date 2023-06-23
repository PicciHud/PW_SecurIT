using Microsoft.EntityFrameworkCore;

namespace CampusWebAPI.Models
{
    public class CodeContext : DbContext
    {
        public CodeContext(DbContextOptions<CodeContext> options)
        : base(options)
        {
        }
        public DbSet<Code> Code { get; set; } = null!;
    }
}