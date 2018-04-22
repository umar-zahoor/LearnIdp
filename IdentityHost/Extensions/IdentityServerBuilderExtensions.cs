using IdentityHost.Services;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityHost.Extensions
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddHostUserStore(this IIdentityServerBuilder builder)
        {
            builder.Services.AddScoped<IHostUserRepository, HostUserRepository>();
            builder.AddProfileService<HostUserProfileService>();

            return builder;
        }
    }
}