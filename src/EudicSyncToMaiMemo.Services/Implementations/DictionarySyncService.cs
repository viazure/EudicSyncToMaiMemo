using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 字典同步服务
    /// </summary>
    public class DictionarySyncService(
        IEudicService eudicService,
        IMaiMemoService maiMemoService,
        ILogger<DictionarySyncService> logger) : IDictionarySyncService
    {
        /// <summary>
        /// 同步词典
        /// </summary>
        /// <returns></returns>
        public async Task SyncDictionariesAsync(CancellationToken stoppingToken)
        {
            // 获取欧路词典词库列表
            //var eudicWords = await eudicService.GetWords();

            //logger.LogInformation("eudicWords: {text}", string.Join(",", eudicWords));

            await maiMemoService.Test();
        }
    }
}
