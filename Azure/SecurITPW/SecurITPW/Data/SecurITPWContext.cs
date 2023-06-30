using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SecurITPW.Models;

namespace SecurITPW.Data
{
    public class SecurITPWContext : DbContext
    {
        public SecurITPWContext (DbContextOptions<SecurITPWContext> options)
            : base(options)
        {
        }

        public DbSet<SecurITPW.Models.Access> Access { get; set; } = default!;

        public DbSet<SecurITPW.Models.House> House { get; set; } = default!;

        public DbSet<SecurITPW.Models.User> User { get; set; } = default!;
    }
}
