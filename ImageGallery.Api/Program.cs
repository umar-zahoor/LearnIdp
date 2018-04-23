using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace ImageGallery.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var seed = false;

            if (args != null && args.Any(_ => string.Equals(_, "/seed", StringComparison.InvariantCultureIgnoreCase)))
            {
                seed = true;
                args = args.Except(new[] { "/seed" }).ToArray();
            }

            var webHost = BuildWebHost(args);

            if (seed)
            {
                SeedData.EnsureSeedData(webHost.Services);
            }

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
