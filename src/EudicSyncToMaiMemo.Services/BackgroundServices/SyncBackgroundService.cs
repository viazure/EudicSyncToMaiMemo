using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.BackgroundServices
{
    /// <summary>
    /// 同步的托管后台服务
    /// </summary>
    public class SyncBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;

        public SyncBackgroundService(ILogger<SyncBackgroundService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("The current time is: {CurrentTime}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
    }
}
