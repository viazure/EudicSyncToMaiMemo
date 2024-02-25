using EudicSyncToMaiMemo.Services.Implementations;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EudicSyncToMaiMemo.Infrastructure.ServiceExtensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddAdditionalServices(this IServiceCollection services)
        {
            services.AddScoped<IEudicService, EudicService>();
            services.AddScoped<IMaiMemoService, MaiMemoService>();
            services.AddScoped<INotificationService, NotificationService>();

            return services;
        }
    }
}
