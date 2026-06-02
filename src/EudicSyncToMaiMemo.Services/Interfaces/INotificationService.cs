namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 同步结果 Webhook 通知
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// 发送同步结果或运行状态通知
        /// </summary>
        /// <param name="message">通知正文，由配置模板中的 {content} 占位符替换而来</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task SendNotification(string message, CancellationToken cancellationToken = default);
    }
}
