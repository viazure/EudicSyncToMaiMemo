using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 字典同步服务
    /// </summary>
    public class DictionarySyncService(
        IEudicService eudicService,
        IMaiMemoService maiMemoService,
        IConfiguration configuration,
        ILogger<DictionarySyncService> logger) : IDictionarySyncService
    {
        /// <summary>
        /// 同步词典
        /// </summary>
        /// <returns></returns>
        public async Task SyncDictionariesAsync(CancellationToken stoppingToken)
        {
            // 获取欧路词典词库列表
            string eudicBookId = configuration.GetSection("Eudic:DefaultBookId").Value ?? "0";
            var eudicWords = await eudicService.GetWords(eudicBookId);

            if (eudicWords.Count == 0)
            {
                throw new Exception("没有获取到欧路词典的单词。");
            }

            // 保存到墨墨云词库
            string maiMemoNotepadId = configuration
                .GetSection("MaiMemo:DefaultNotepadId")
                .Value ?? throw new Exception("没有配置默认的云词库 ID。");

            await maiMemoService.SyncToMaimemoNotepad(maiMemoNotepadId, eudicWords);

            logger.LogInformation("同步完成。");

        }
    }
}
