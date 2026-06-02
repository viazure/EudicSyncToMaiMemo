using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// POST /notepads/{id} 更新请求体中的 notepad 节点
    /// </summary>
    public sealed class NotepadUpdateDto
    {
        /// <summary>
        /// 发布状态
        /// </summary>
        [JsonProperty("status")]
        public string Status { get; set; } = "PUBLISHED";

        /// <summary>
        /// 词表 content
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
        public List<string> Tags { get; set; } = [];
    }
}
