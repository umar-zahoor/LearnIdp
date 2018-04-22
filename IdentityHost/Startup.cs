using System.Reflection;
using IdentityHost.Entities;
using IdentityHost.Extensions;
using IdentityHost.Services;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityHost
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        // For EF Migrations
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var userDataConnectionString = Configuration.GetConnectionString("UserAccountConnection");
            var identityDataConnectionString = Configuration.GetConnectionString("IdentityDbConnection");
            services.AddDbContext<HostUserContext>(_ => _.UseSqlServer(userDataConnectionString));

            services.AddScoped<IHostUserRepository, HostUserRepository>();
            services.AddTransient<TestConfiguration>();

            services.AddMvc();

            //var testConfig = new TestConfiguration(Configuration);

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddHostUserStore()

                // This adds the config data from DB (Clients, Resources, CORS)
                .AddConfigurationStore(options =>
                {
                    options.ResolveDbContextOptions = (provider, builder) =>
                    {
                        builder.UseSqlServer(identityDataConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                })

                // This adds the operational data from DB (Codes, Tokens, Consents)
                .AddOperationalStore(options =>
                {
                    options.ResolveDbContextOptions = (provider, builder) =>
                    {
                        builder.UseSqlServer(identityDataConnectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                    };
                });

            // Development code
            //.AddInMemoryClients(testConfig.GetClients())
            //.AddInMemoryIdentityResources(testConfig.GetIdentityResources())
            //.AddInMemoryApiResources(testConfig.GetApiResources())
            //.AddInMemoryPersistedGrants();
            //.AddTestUsers(testConfig.GetTestUsers());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}