namespace EudicSyncToMaiMemo.Models.DTOs.Notification
{
    /// <summary>
    /// 消息通知 DTO
    /// </summary>
    public class NotificationMessageDto
    {
        /// <summary>
        /// 通知消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 同步的单词总数量
        /// </summary>
        public int Total { get; set; }
    }
}
