using IdentityHost.Entities;
using IdentityHost.Services;
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
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<HostUserContext>(_ => _.UseSqlServer(connectionString));

            services.AddScoped<IHostUserRepository, HostUserRepository>();
            
            services.AddMvc();
            
            var testConfig = new TestConfiguration(Configuration);

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryClients(testConfig.GetClients())
                .AddInMemoryIdentityResources(testConfig.GetIdentityResources())
                .AddInMemoryApiResources(testConfig.GetApiResources())
                .AddInMemoryPersistedGrants()
                .AddTestUsers(testConfig.GetTestUsers());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, HostUserContext hostUserContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            hostUserContext.Database.Migrate();
            hostUserContext.EnsureSeedDataForContext();
            
            app.UseIdentityServer();

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();
        }
    }
}
