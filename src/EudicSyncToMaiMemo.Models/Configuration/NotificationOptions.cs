namespace EudicSyncToMaiMemo.Models.Configuration
{
    /// <summary>
    /// 同步结果 Webhook 通知配置
    /// </summary>
    public sealed class NotificationOptions
    {
        /// <summary>
        /// 配置节名称，对应 appsettings 中 Notification 节点
        /// </summary>
        public const string SectionName = "Notification";

        /// <summary>
        /// 是否启用通知
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// 通知 URL 模板，可含 {content} 占位符
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// POST 请求体 JSON 字符串，为空时使用 GET
        /// </summary>
        public string RequestBody { get; set; } = string.Empty;

        /// <summary>
        /// 请求头，格式 key1=value1;key2=value2
        /// </summary>
        public string Headers { get; set; } = string.Empty;
    }
}
