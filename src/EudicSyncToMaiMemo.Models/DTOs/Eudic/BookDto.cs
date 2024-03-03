using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路词典生词本数据传输对象
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// 生词本 ID，默认为 0
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; } = "0";

        /// <summary>
        /// 生词本语言，默认为英语（en）
        /// </summary>
        [JsonProperty("language")]
        public string Language { get; set; } = "en";

        /// <summary>
        /// 生词本名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;
    }
}
