using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 欧路生词本到墨墨云词库的同步编排
    /// </summary>
    public sealed class DictionarySyncService(
        IEudicService eudicService,
        IMaiMemoService maiMemoService,
        IOptions<EudicOptions> eudicOptions,
        IOptions<MaiMemoOptions> maiMemoOptions,
        INotificationService notificationService,
        ILogger<DictionarySyncService> logger) : IDictionarySyncService
    {
        /// <inheritdoc />
        public async Task SyncDictionaries(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            string eudicBookId = eudicOptions.Value.DefaultBookId;
            var eudicWords = await eudicService.GetWords(eudicBookId, stoppingToken);

            if (eudicWords.Count == 0)
            {
                logger.LogInformation("欧路生词本为空，跳过同步");
                await notificationService.SendNotification(
                    "欧路生词本为空，没有需要同步的单词",
                    stoppingToken);
                return;
            }

            string maiMemoNotepadId = maiMemoOptions.Value.DefaultNotepadId;
            await maiMemoService.SyncToMaimemoNotepad(maiMemoNotepadId, eudicWords, stoppingToken);

            logger.LogInformation("同步完成");
        }
    }
}
