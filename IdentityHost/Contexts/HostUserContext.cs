using IdentityHost.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityHost.Contexts
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
