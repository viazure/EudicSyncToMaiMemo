using System.ComponentModel.DataAnnotations;

namespace EudicSyncToMaiMemo.Models.Configuration
{
    /// <summary>
    /// 欧路词典 Open API 接入配置
    /// </summary>
    public sealed class EudicOptions
    {
        /// <summary>
        /// 配置节名称，对应 appsettings 中 Eudic 节点
        /// </summary>
        public const string SectionName = "Eudic";

        /// <summary>
        /// Authorization 请求头值，来自欧路开放平台
        /// </summary>
        [Required(ErrorMessage = "未设置欧路词典授权信息（Authorization）")]
        public string Authorization { get; set; } = string.Empty;

        /// <summary>
        /// 默认同步的生词本 ID，0 表示默认生词本
        /// </summary>
        public string DefaultBookId { get; set; } = "0";
    }
}
