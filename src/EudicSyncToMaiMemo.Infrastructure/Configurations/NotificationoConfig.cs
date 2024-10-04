namespace EudicSyncToMaiMemo.Infrastructure.Configurations
{
    /// <summary>
    /// 通知配置
    /// </summary>
    public class NotificationConfig
    {
        /// <summary>
        /// 是否启用通知功能
        /// </summary>
        public bool Enabled { get; set; }
        /// <summary>
        /// 通知服务地址
        /// </summary>
        public required string Url { get; set; }
        /// <summary>
        /// 通知服务请求体
        /// </summary>
        public string? RequestBody { get; set; }
        /// <summary>
        /// 通知服务请求头
        /// </summary>
        public string? Headers { get; set; }
    }
}
