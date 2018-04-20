using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using ImageGallery.Client.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Newtonsoft.Json.Linq;

namespace ImageGallery.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            // Add Authorization Policies
            services.AddAuthorization(authzOptions =>
            {
                authzOptions.AddPolicy(
                    "CanOrderFrame",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.RequireClaim("country", "in");
                        policyBuilder.RequireClaim("subscription", "paiduser");
                    });
            });

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = "Cookies";
                    options.DefaultChallengeScheme = "oidc";
                })
                .AddCookie("Cookies", options => { options.AccessDeniedPath = "/Authorization/AccessDenied"; })
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    options.SaveTokens = true;

                    options.Authority = "https://localhost:44332/";
                    options.RequireHttpsMetadata = true;

                    options.ClientId = "ImageGallery-WebApp";
                    options.ClientSecret = "secret";

                    //Ensure middleware calls IDP for user claims
                    options.GetClaimsFromUserInfoEndpoint = true;
                    options.ResponseType = OpenIdConnectResponseType.CodeIdToken;

                    options.Events = new OpenIdConnectEvents
                    {
                        OnUserInformationReceived = SetUserInformationReceived,
                        OnTokenValidated = TransformClaims
                    };

                    options.Scope.Add("openid");
                    options.Scope.Add(JwtClaimTypes.Profile);
                    options.Scope.Add(JwtClaimTypes.Address);
                    options.Scope.Add("user-role");
                    options.Scope.Add("imagegallery-api");
                    options.Scope.Add("subscription");
                    options.Scope.Add("country");
                });

            // register an IHttpContextAccessor so we can access the current
            // HttpContext in services by injecting it
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // register an IImageGalleryHttpClient
            services.AddScoped<IImageGalleryHttpClient, ImageGalleryHttpClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Gallery}/{action=Index}/{id?}");
            });
        }


        private static Task TransformClaims(TokenValidatedContext context)
        {
            var identity = context.Principal.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var subjectClaim = identity.Claims.FirstOrDefault(_ => _.Type == JwtClaimTypes.Subject);
                var claimsIdentity = new ClaimsIdentity(identity.AuthenticationType, JwtClaimTypes.GivenName,
                    JwtClaimTypes.Role);

                claimsIdentity.AddClaim(subjectClaim);

                context.Principal = new ClaimsPrincipal(claimsIdentity);
            }

            return Task.CompletedTask;
        }


        // Used to set proper user claims
        private static Task SetUserInformationReceived(UserInformationReceivedContext context)
        {
            var claims = new List<Claim>();
            foreach (var jToken in context.User.Children())
            {
                JToken roles;
                if (context.User.TryGetValue(jToken.Path, value: out roles))
                {
                    if (roles.Type != JTokenType.Array)
                        claims.Add(new Claim(roles.Path, roles.ToString()));

                    else
                        claims.AddRange(jToken.Select(role => new Claim(role.Path, role.ToString())));
                }
            }

            (context.Principal.Identity as ClaimsIdentity)?.AddClaims(claims);

            return Task.CompletedTask;
        }
    }
}