using EudicSyncToMaiMemo.Services.Interfaces;
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
        ILogger<SyncBackgroundService> logger) : BackgroundService
    {
        private const string ClassName = nameof(SyncBackgroundService);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
            "{Name} is running.", ClassName);

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
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);
                Environment.Exit(1);
            }
        }


        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
            "{Name} is working.", ClassName);

            using (IServiceScope scope = serviceScopeFactory.CreateScope())
            {
                var dictionarySyncService =
                    scope.ServiceProvider.GetRequiredService<IDictionarySyncService>();

                await dictionarySyncService.SyncDictionariesAsync(stoppingToken);
            }
        }


        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation(
                "{Name} is stopping.", ClassName);

            await base.StopAsync(stoppingToken);
        }
    }
}
