using System;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityHost.Extensions
{
    public static class ConfigurationDbContextExtensions
    {
        public static void EnsureSeedDataForContext(this ConfigurationDbContext
            context, IServiceProvider serviceProvider)
        {
            var configuration = serviceProvider.GetRequiredService<TestConfiguration>();

            if (!context.Clients.Any())
            {
                foreach (var client in configuration.GetClients())
                {
                    context.Clients.Add(client.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in configuration.GetIdentityResources())
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }

                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var apiResource in configuration.GetApiResources())
                {
                    context.ApiResources.Add(apiResource.ToEntity());
                }

                context.SaveChanges();
            }
        }
    }
}
