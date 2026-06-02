namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 欧路生词本到墨墨云词库的同步编排
    /// </summary>
    public interface IDictionarySyncService
    {
        /// <summary>
        /// 执行一次完整同步流程
        /// </summary>
        ///
        /// <remarks>
        /// 欧路生词本为空时记录日志并通知，不抛出异常
        /// </remarks>
        /// <param name="stoppingToken">取消令牌</param>
        Task SyncDictionaries(CancellationToken stoppingToken);
    }
}
