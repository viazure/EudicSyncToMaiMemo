﻿using EudicSyncToMaiMemo.Services.Interfaces;
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

                    // 一周同步一次
                    await Task.Delay(TimeSpan.FromDays(7), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
                await notificationService.SendNotification("同步服务已停止。");
                logger.LogInformation("同步服务已停止。");
            }
            catch (InvalidOperationException ex)
            {
                await notificationService.SendNotification(ex.Message);
                logger.LogError(ex, "同步失败：{Message}", ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "同步服务出现异常: {Message}", ex.InnerException?.Message ?? ex.Message);
            }
        }


        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                var dictionarySyncService =
                    scope.ServiceProvider.GetRequiredService<IDictionarySyncService>();

                await dictionarySyncService.SyncDictionaries(stoppingToken);
            }
        }


        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation("Sync Services is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
