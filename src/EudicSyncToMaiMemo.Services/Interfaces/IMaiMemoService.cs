namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 墨墨云词库同步服务
    /// </summary>
    public interface IMaiMemoService
    {
        /// <summary>
        /// 将欧路词表增量合并到指定云词库
        /// </summary>
        /// <param name="notepadId">目标云词库 ID</param>
        /// <param name="eudicWords">欧路生词本单词列表</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task SyncToMaimemoNotepad(
            string notepadId,
            List<string> eudicWords,
            CancellationToken cancellationToken = default);
    }
}
