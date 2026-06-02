using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// 云词本完整实体
    /// </summary>
    public sealed class NotepadDto
    {
        /// <summary>
        /// 云词本 ID，np- 前缀
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 类型，FAVORITE 或 NOTEPAD
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 创建者用户 ID，部分响应中省略
        /// </summary>
        [JsonProperty("creator")]
        public int? Creator { get; set; }

        /// <summary>
        /// 发布状态，PUBLISHED / UNPUBLISHED / DELETED
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 原始词表内容，一行一词，井号开头为章节
        /// </summary>
        [JsonProperty("content")]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 简介
        /// </summary>
        [JsonProperty("brief")]
        public string Brief { get; set; } = string.Empty;

        /// <summary>
        /// 标签列表
        /// </summary>
        [JsonProperty("tags")]
        public List<string>? Tags { get; set; }

        /// <summary>
        /// content 解析结果，同步逻辑仅使用 content 字段
        /// </summary>
        [JsonProperty("list")]
        public List<NotepadParsedItemDto>? List { get; set; }

        /// <summary>
        /// 创建时间，ISO 8601
        /// </summary>
        [JsonProperty("created_time")]
        public string CreatedTime { get; set; } = string.Empty;

        /// <summary>
        /// 更新时间，ISO 8601
        /// </summary>
        [JsonProperty("updated_time")]
        public string UpdatedTime { get; set; } = string.Empty;
    }
}
