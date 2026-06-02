using System.ComponentModel.DataAnnotations;

namespace EudicSyncToMaiMemo.Models.Configuration
{
    /// <summary>
    /// 墨墨背单词云词库同步配置
    /// </summary>
    public sealed class MaiMemoOptions
    {
        /// <summary>
        /// 配置节名称，对应 appsettings 中 MaiMemo 节点
        /// </summary>
        public const string SectionName = "MaiMemo";

        /// <summary>
        /// 墨墨接入方式，OpenApi 或 Legacy
        /// </summary>
        public MaiMemoProvider Provider { get; set; } = MaiMemoProvider.OpenApi;

        /// <summary>
        /// 开放 API Token，Provider 为 OpenApi 时必填
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 登录用户名，Provider 为 Legacy 时必填
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 登录密码，Provider 为 Legacy 时必填
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// 默认同步的目标云词库 ID
        /// </summary>
        ///
        /// <remarks>
        /// Legacy 模式填网页 URL 中的数字 ID；OpenApi 模式填 GET /notepads 返回的 np- 开头 ID
        /// </remarks>
        [Required(ErrorMessage = "没有配置默认的云词库 ID")]
        public string DefaultNotepadId { get; set; } = string.Empty;

        /// <summary>
        /// OpenApi 模式下按标题解析云词库，与网页版数字 ID 二选一
        /// </summary>
        ///
        /// <remarks>
        /// 当 DefaultNotepadId 不是 np- 开头时，将拉取词库列表并按标题精确匹配
        /// </remarks>
        public string? DefaultNotepadTitle { get; set; }
    }
}
