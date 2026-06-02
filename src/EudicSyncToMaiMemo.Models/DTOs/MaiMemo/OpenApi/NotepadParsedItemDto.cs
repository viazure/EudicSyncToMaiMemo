using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// content 解析条目，实际 API 为扁平结构（chapter/word 与 type 同级）
    /// </summary>
    public sealed class NotepadParsedItemDto
    {
        /// <summary>
        /// 条目类型，CHAPTER / WORD / DRAFT_WORD 等
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = string.Empty;

        /// <summary>
        /// 所属章节名
        /// </summary>
        [JsonProperty("chapter")]
        public string? Chapter { get; set; }

        /// <summary>
        /// 单词拼写，仅 WORD / DRAFT_WORD 时有值
        /// </summary>
        [JsonProperty("word")]
        public string? Word { get; set; }
    }
}
