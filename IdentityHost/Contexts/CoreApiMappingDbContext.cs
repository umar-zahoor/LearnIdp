using IdentityHost.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityHost.Contexts
{
    public class CoreApiMappingDbContext : DbContext
    {
        public CoreApiMappingDbContext(DbContextOptions<CoreApiMappingDbContext> options)
            : base(options)
        {
            
        }

        public DbSet<ApiMapping> ApiMappings { get; set; }
    }
}