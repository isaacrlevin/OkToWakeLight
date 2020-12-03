using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OkToWake.Models;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OkToWake
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            var sp = host.Services.GetService<IServiceScopeFactory>()
                .CreateScope()
                .ServiceProvider;
            var options = sp.GetRequiredService<DbContextOptions<OkToWakeContext>>();
            await EnsureDbCreatedAndSeedWithCountOfAsync(options);

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                config.AddEnvironmentVariables();
            })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static async Task EnsureDbCreatedAndSeedWithCountOfAsync(DbContextOptions<OkToWakeContext> options)
        {
            var factory = new LoggerFactory();
            var builder = new DbContextOptionsBuilder<OkToWakeContext>(options)
                .UseLoggerFactory(factory);

            using var context = new OkToWakeContext(builder.Options);
            if (await context.Database.EnsureCreatedAsync()) { }
        }
    }
}
