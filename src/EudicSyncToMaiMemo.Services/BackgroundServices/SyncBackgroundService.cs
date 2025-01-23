using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.BackgroundServices
{
    /// <summary>
    /// 同步的托管后台服务
    /// </summary>
    public class SyncBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        INotificationService notificationService,
        IConfiguration configuration,
        ILogger<SyncBackgroundService> logger) : BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("同步服务启动。");

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await DoWorkAsync(stoppingToken);

                    int interval = configuration.GetValue<int>("SyncInterval");
                    await Task.Delay(TimeSpan.FromDays(interval), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                await notificationService.SendNotification("同步服务已手动停止。");
                logger.LogInformation("同步服务已手动停止。");
            }
            catch (NotificationException ex)
            {
                logger.LogError(ex, "通知服务异常：{Message}", ex.Message);
            }
            catch (ConfigurationException ex)
            {
                await notificationService.SendNotification($"配置异常：{ex.Message}");
                logger.LogError(ex, "配置异常：{Message}", ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                await notificationService.SendNotification($"操作异常：{ex.Message}");
                logger.LogError(ex, "操作异常：{Message}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "同步服务出现异常: {Message}", ex.InnerException?.Message ?? ex.Message);
            }
        }


        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var dictionarySyncService = scope.ServiceProvider.GetRequiredService<IDictionarySyncService>();

            await dictionarySyncService.SyncDictionaries(stoppingToken);
        }


        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Sync Services is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
