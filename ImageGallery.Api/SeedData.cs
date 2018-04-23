using System;
using ImageGallery.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ImageGallery.Api
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var galleryContext = scope.ServiceProvider.GetRequiredService<GalleryContext>();

                galleryContext.Database.Migrate();
                galleryContext.EnsureSeedDataForContext();
            }
        }


    }
}