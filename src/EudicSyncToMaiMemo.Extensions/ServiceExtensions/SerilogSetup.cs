using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace EudicSyncToMaiMemo.Extensions.ServiceExtensions
{
    public static class SerilogSetup
    {
        public static IServiceCollection AddSerilogSetup(this IServiceCollection services, IConfiguration configuration)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var logger = loggerConfiguration.CreateLogger();
            services.AddSerilog(logger);

            return services;
        }
    }
}
