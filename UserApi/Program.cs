using System;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Loki;

namespace UserApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();
            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(15);
                    });
                })
                .UseSerilog()
                .UseServiceProviderFactory(new AutofacServiceProviderFactory());
        }
        
        public static void ConfigureLogging()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile(
                    $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
                    true)
                .AddEnvironmentVariables()
                .Build();

            var lokiCredentials = new NoAuthCredentials(configuration.GetSection("Logging").GetSection("Loki")["Uri"]);
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                .WriteTo.Console()
                .WriteTo.LokiHttp(lokiCredentials)
                .Enrich.WithProperty("Environment", environment)
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
    }
}