using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using EudicSyncToMaiMemo.Models.Configuration;

namespace EudicSyncToMaiMemo.Services.BackgroundServices
{
    /// <summary>
    /// 自托管循环同步，按 SyncOptions.IntervalDays 定时执行
    /// </summary>
    public sealed class SyncBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        INotificationService notificationService,
        IOptions<SyncOptions> syncOptions,
        ILogger<SyncBackgroundService> logger) : BackgroundService
    {
        /// <summary>
        /// 启动后立即同步，之后按 IntervalDays 循环
        /// </summary>
        /// <param name="stoppingToken">宿主停止时用于取消等待与同步</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("同步服务启动");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await RunSyncOnceAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    await TrySendStopNotificationAsync();
                    logger.LogInformation("同步服务已手动停止");
                    break;
                }
                catch (NotificationException ex)
                {
                    logger.LogError(ex, "通知服务异常：{Message}", ex.Message);
                }
                catch (ConfigurationException ex)
                {
                    await notificationService.SendNotification($"配置异常：{ex.Message}", stoppingToken);
                    logger.LogError(ex, "配置异常：{Message}", ex.Message);
                }
                catch (InvalidOperationException ex)
                {
                    await notificationService.SendNotification($"操作异常：{ex.Message}", stoppingToken);
                    logger.LogError(ex, "操作异常：{Message}", ex.Message);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "同步服务出现异常：{Message}", ex.InnerException?.Message ?? ex.Message);
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                int intervalDays = syncOptions.Value.IntervalDays;
                logger.LogInformation("下次同步将在 {IntervalDays} 天后执行", intervalDays);
                await Task.Delay(TimeSpan.FromDays(intervalDays), stoppingToken);
            }
        }

        private async Task RunSyncOnceAsync(CancellationToken stoppingToken)
        {
            using IServiceScope scope = serviceScopeFactory.CreateScope();
            var dictionarySyncService = scope.ServiceProvider.GetRequiredService<IDictionarySyncService>();
            await dictionarySyncService.SyncDictionaries(stoppingToken);
        }

        private async Task TrySendStopNotificationAsync()
        {
            try
            {
                await notificationService.SendNotification("同步服务已手动停止", CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "停止通知发送失败");
            }
        }

        /// <summary>
        /// 记录停止日志并等待基类收尾
        /// </summary>
        /// <param name="stoppingToken">停止取消令牌</param>
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("同步服务正在停止");
            await base.StopAsync(stoppingToken);
        }
    }
}
