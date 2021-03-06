﻿using System.Collections.Generic;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using Microsoft.Extensions.Configuration;

namespace IdentityHost
{
    internal class TestConfiguration
    {
        public TestConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        internal List<TestUser> GetTestUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "d860efca-22d9-47fd-8249-791ba61b07c7",
                    Username = "umar",
                    IsActive = true,
                    Password = "umar",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Umar"),
                        new Claim(JwtClaimTypes.Address, "Home Street, Home City"),
                        new Claim(JwtClaimTypes.Role, "paiduser"),
                        new Claim("country", "in"),
                        new Claim("subscription", "paiduser"),
                    }
                },
                
                new TestUser
                {
                    SubjectId = "b7539694-97e7-4dfe-84da-b4256e1ff5c7",
                    Username = "test",
                    IsActive = true,
                    Password = "test",
                    Claims = new List<Claim>
                    {
                        new Claim(JwtClaimTypes.GivenName, "Test User"),
                        new Claim(JwtClaimTypes.Address, "1 Street, Some City"),
                        new Claim(JwtClaimTypes.Role, "freeuser"),
                        new Claim("country", "us"),
                        new Claim("subscription", "freeuser"),
                    }
                }
            };
        }

        internal IEnumerable<Client> GetClients()
        {
            var clientUri = Configuration.GetValue<string>("AppSettings:Client:Uri");
            
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Image Gallery",

                    ClientId = "ImageGallery-WebApp",
                    ClientSecrets = new List<Secret> {new Secret("secret".Sha256())},
                    
                    // For enabling refresh tokens
                    AllowOfflineAccess = true,

                    AllowedGrantTypes = GrantTypes.Hybrid,
                    RedirectUris = { $"{clientUri}signin-oidc" },
                    PostLogoutRedirectUris = { $"{clientUri}signout-callback-oidc" },

                    // Lifetime in seconds
                    // IdentityTokenLifetime = 300,
                    // AuthorizationCodeLifetime = 300,
                    AccessTokenLifetime = 120,

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        
                        "user-role",
                        "imagegallery-api",
                        "country",
                        "subscription",
                        
                        // For refresh tokens
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                    },

                    // Use UserInfo end-point instead
                    //AlwaysIncludeUserClaimsInIdToken = true
                    
                    // For enabling reference tokens
                    AccessTokenType = AccessTokenType.Reference
                }
            };
        }

        internal IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource("user-role", "Your role(s)", new []{ JwtClaimTypes.Role }),
                new IdentityResource("country", "The country you're living in", new []{ "country" }),
                new IdentityResource("subscription", "Your subscriptions", new []{ "subscription" }),
            };
        }

        internal IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("imagegallery-api", "ImageGallery API",
                    // Include user claims in access token
                    new []{ "role" })
                {
                    // For using reference tokens, we have to enable introspection endpoint.
                    // which requires an api secret
                    ApiSecrets = { new Secret("apisecret".Sha256()) }
                }
            };
        }
    }
}