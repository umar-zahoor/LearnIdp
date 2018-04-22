using System;
using IdentityHost.Entities;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityHost
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var hostUserContext = scope.ServiceProvider.GetRequiredService<HostUserContext>();

                hostUserContext.Database.Migrate();
                hostUserContext.EnsureSeedDataForContext();

                var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                configurationDbContext.Database.Migrate();
                configurationDbContext.EnsureSeedDataForContext(serviceProvider);

                var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();

                persistedGrantDbContext.Database.Migrate();
            }
        }


    }
}