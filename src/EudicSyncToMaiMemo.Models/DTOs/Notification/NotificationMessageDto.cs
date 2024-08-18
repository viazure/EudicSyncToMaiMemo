namespace EudicSyncToMaiMemo.Models.DTOs.Notification
{
    /// <summary>
    /// 消息通知 DTO
    /// </summary>
    public class NotificationMessageDto
    {
        /// <summary>
        /// 标题
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; } = string.Empty;
    }
}
