using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路生词本单词条目
    /// </summary>
    public class WordDto
    {
        /// <summary>
        /// 单词拼写
        /// </summary>
        [JsonProperty("word")]
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// 音标
        /// </summary>
        [JsonProperty("phon")]
        public string Phon { get; set; } = string.Empty;

        /// <summary>
        /// 释义
        /// </summary>
        [JsonProperty("exp")]
        public string Exp { get; set; } = string.Empty;

        /// <summary>
        /// 加入生词本时间，ISO 8601
        /// </summary>
        [JsonProperty("add_time")]
        public string AddTime { get; set; } = string.Empty;

        /// <summary>
        /// 单词等级
        /// </summary>
        [JsonProperty("star")]
        public int Star { get; set; }

        /// <summary>
        /// 单词所在语境例句
        /// </summary>
        [JsonProperty("context_line")]
        public string ContextLine { get; set; } = string.Empty;
    }
}
