namespace EudicSyncToMaiMemo.Models.Configuration
{
    /// <summary>
    /// 自托管循环同步间隔配置
    /// </summary>
    public sealed class SyncOptions
    {
        /// <summary>
        /// 配置节名称，对应 appsettings 中 Sync 节点
        /// </summary>
        public const string SectionName = "Sync";

        /// <summary>
        /// 两次同步之间的间隔天数，仅 Hosted 模式生效
        /// </summary>
        public int IntervalDays { get; set; } = 7;
    }
}
