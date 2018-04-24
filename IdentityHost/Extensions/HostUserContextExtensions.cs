using System.Collections.Generic;
using System.Linq;
using IdentityHost.Contexts;
using IdentityHost.Entities;
using IdentityModel;

namespace IdentityHost.Extensions
{
    public static class HostUserContextExtensions
    {
        public static void EnsureSeedDataForContext(this HostUserContext context)
        {
            //// Add 2 demo users if there aren't any users yet
            if (context.Users.Any())
            {
                return;
            }

            // init users
            var users = new List<User>()
            {
                new User
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "umar",
                    IsActive = true,
                    Password = "umar",
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.GivenName, "Umar"),
                        new UserClaim(JwtClaimTypes.Address, "Home Street, Home City"),
                        new UserClaim(JwtClaimTypes.Role, "paiduser"),
                        new UserClaim("country", "in"),
                        new UserClaim("subscription", "paiduser"),
                    }
                },
                
                new User
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "test",
                    IsActive = true,
                    Password = "test",
                    Claims = new List<UserClaim>
                    {
                        new UserClaim(JwtClaimTypes.GivenName, "Test User"),
                        new UserClaim(JwtClaimTypes.Address, "1 Street, Some City"),
                        new UserClaim(JwtClaimTypes.Role, "freeuser"),
                        new UserClaim("country", "us"),
                        new UserClaim("subscription", "freeuser"),
                    }
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
