using Microsoft.Extensions.Hosting;
using Serilog;

namespace EudicSyncToMaiMemo.Infrastructure.ServiceExtensions
{
    public static class SerilogSetup
    {
        public static IHostBuilder AddSerilogSetup(this IHostBuilder builder)
        {
            builder.ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console();

                Log.Logger = loggerConfiguration.CreateLogger();
            });

            builder.UseSerilog();

            return builder;
        }
    }
}
