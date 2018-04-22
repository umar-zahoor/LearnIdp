using Microsoft.EntityFrameworkCore;

namespace IdentityHost.Entities
{
    public class HostUserContext : DbContext
    {
        public HostUserContext(DbContextOptions<HostUserContext> options)
           : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
