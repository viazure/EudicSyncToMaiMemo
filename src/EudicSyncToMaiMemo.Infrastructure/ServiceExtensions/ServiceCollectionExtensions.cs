using EudicSyncToMaiMemo.Services.BackgroundServices;
using EudicSyncToMaiMemo.Services.Implementations;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace EudicSyncToMaiMemo.Infrastructure.ServiceExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IHostBuilder AddServices(this IHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                // 注册 SyncBackgroundService 作为托管服务
                services.AddHostedService<SyncBackgroundService>();

                // 服务接口与实现的映射
                services.AddScoped<IEudicService, EudicService>();
                services.AddScoped<IMaiMemoService, MaiMemoService>();
                services.AddScoped<INotificationService, NotificationService>();

                // 可以使用 configuration 来配置后面注册的服务
                var configuration = context.Configuration;
            });

            return builder;
        }
    }
}
