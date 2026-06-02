using EudicSyncToMaiMemo.Extensions.Configuration;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Services.Implementations;
using EudicSyncToMaiMemo.Services.Implementations.MaiMemo;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Extensions.ServiceExtensions
{
    /// <summary>
    /// 应用服务与配置注册扩展
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 注册同步相关服务、Options 与 HttpClient
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configuration">应用配置</param>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddOptions<EudicOptions>()
                .BindConfiguration(EudicOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddOptions<MaiMemoOptions>()
                .BindConfiguration(MaiMemoOptions.SectionName)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            services.AddSingleton<IValidateOptions<MaiMemoOptions>, MaiMemoOptionsValidator>();

            services.AddSingleton<IPostConfigureOptions<SyncOptions>, SyncOptionsPostConfigure>();

            services.AddOptions<SyncOptions>()
                .BindConfiguration(SyncOptions.SectionName)
                .ValidateOnStart();

            services.AddOptions<NotificationOptions>()
                .BindConfiguration(NotificationOptions.SectionName)
                .ValidateOnStart();

            services.AddHttpClient();
            services.AddSingleton<IHttpHelper, HttpHelper>();

            services.AddScoped<IEudicService, EudicService>();
            services.AddScoped<MaiMemoOpenApiService>();
            services.AddScoped<MaiMemoLegacyWebService>();
            services.AddScoped<IMaiMemoService>(sp =>
            {
                MaiMemoOptions options = sp.GetRequiredService<IOptions<MaiMemoOptions>>().Value;

                return options.Provider switch
                {
                    MaiMemoProvider.OpenApi => sp.GetRequiredService<MaiMemoOpenApiService>(),
                    MaiMemoProvider.Legacy => sp.GetRequiredService<MaiMemoLegacyWebService>(),
                    _ => throw new InvalidOperationException($"不支持的墨墨接入方式：{options.Provider}")
                };
            });
            services.AddScoped<IDictionarySyncService, DictionarySyncService>();
            services.AddSingleton<INotificationService, NotificationService>();

            return services;
        }
    }
}
