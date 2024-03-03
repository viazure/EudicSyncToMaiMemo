using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路词典单词数据传输对象
    /// </summary>
    public class WordDto
    {
        /// <summary>
        /// 单词
        /// </summary>
        [JsonProperty("word")]
        public string Word { get; set; } = string.Empty;

        /// <summary>
        /// 释义
        /// </summary>
        [JsonProperty("exp")]
        public string Exp { get; set; } = string.Empty;
    }
}
