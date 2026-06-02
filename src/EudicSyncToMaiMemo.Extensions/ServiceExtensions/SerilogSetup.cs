using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace EudicSyncToMaiMemo.Extensions.ServiceExtensions
{
    /// <summary>
    /// Serilog 注册，避免与默认 Console 日志重复输出
    /// </summary>
    public static class SerilogSetup
    {
        /// <summary>
        /// 从配置读取 Serilog，并替换默认 <see cref="ILogger"/> 提供程序
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">应用配置</param>
        public static IServiceCollection AddSerilogSetup(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddLogging(logging => logging.ClearProviders());

            services.AddSerilog((_, loggerConfiguration) => loggerConfiguration
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext());

            return services;
        }
    }
}
