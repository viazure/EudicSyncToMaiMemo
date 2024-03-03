using EudicSyncToMaiMemo.Services.Implementations;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EudicSyncToMaiMemo.Extensions.ServiceExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdditionalServices(this IServiceCollection services)
        {
            services.AddScoped<IEudicService, EudicService>();
            services.AddScoped<IMaiMemoService, MaiMemoService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDictionarySyncService, DictionarySyncService>();

            return services;
        }
    }
}
