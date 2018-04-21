using AutoMapper;
using ImageGallery.Api.Authorization;
using ImageGallery.Api.Helpers;
using ImageGallery.API.Entities;
using ImageGallery.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImageGallery.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var authority = Configuration.GetValue<string>("AppSettings:Idp:Uri");
            var useSsl = Configuration.GetValue<bool>("AppSettings:Idp:UseSsl");
            
            services.AddMvc();

            services.AddAuthorization(authzOptions =>
            {
                authzOptions.AddPolicy(
                    "MustOwnImage",
                    policyBuilder =>
                    {
                        policyBuilder.RequireAuthenticatedUser();
                        policyBuilder.AddRequirements(new MustOwnImageRequirement());
                    });
            });

            // register the DbContext on the container, getting the connection string from
            // appSettings (note: use this during development; in a production environment,
            // it's better to store the connection string in an environment variable)
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<GalleryContext>(o => o.UseSqlServer(connectionString));

            // register the repository
            services.AddScoped<IGalleryRepository, GalleryRepository>();
            services.AddAutoMapper(typeof(MapperProfile));

            services.AddScoped<IAuthorizationHandler, MustOwnImageHandler>();


            //services.AddAuthorization();

            services.AddAuthentication("Bearer")
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = $"{authority}";
                    options.RequireHttpsMetadata = useSsl;
                    options.ApiName = "imagegallery-api";
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            GalleryContext galleryContext)
        {
            loggerFactory.AddConsole();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            // ensure DB migrations are applied
            galleryContext.Database.Migrate();

            // seed the DB with data
            galleryContext.EnsureSeedDataForContext();


            app.UseAuthentication();

            app.UseMvc();
        }
    }
}