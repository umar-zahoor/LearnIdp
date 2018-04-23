using System;
using System.Linq;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace IdentityHost
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var seed = false;

            if (args != null && args.Any(_ => string.Equals(_, "/seed", StringComparison.InvariantCultureIgnoreCase)))
            {
                seed = true;
                args = args.Except(new[] {"/seed"}).ToArray();
            }

            var webHost = BuildWebHost(args);

            if (seed)
            {
                SeedData.EnsureSeedData(webHost.Services);
            }

            webHost.Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File($@"ServerLogs\{DateTime.Today:dd-MM-yyyy}\idp_log.txt")
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}",
                    theme: AnsiConsoleTheme.Literate)
                .CreateLogger();

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseIISIntegration()
                .ConfigureLogging(cfg =>
                {
                    cfg.ClearProviders();
                    cfg.AddSerilog();
                })
                .Build();
        }
    }
}
