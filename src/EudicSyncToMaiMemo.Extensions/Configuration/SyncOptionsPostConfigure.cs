using EudicSyncToMaiMemo.Models.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Extensions.Configuration
{
    /// <summary>
    /// 兼容根节点 SyncInterval 旧配置键
    /// </summary>
    public sealed class SyncOptionsPostConfigure(IConfiguration configuration) : IPostConfigureOptions<SyncOptions>
    {
        /// <summary>
        /// 当 Sync 节缺失时从 SyncInterval 回填 IntervalDays
        /// </summary>
        /// <param name="name">Options 注册名称</param>
        /// <param name="options">待补全配置</param>
        public void PostConfigure(string? name, SyncOptions options)
        {
            int? syncSectionInterval = configuration.GetValue<int?>("Sync:IntervalDays");
            if (syncSectionInterval is > 0)
            {
                return;
            }

            int legacyInterval = configuration.GetValue("SyncInterval", 0);
            if (legacyInterval > 0)
            {
                options.IntervalDays = legacyInterval;
            }
        }
    }
}
