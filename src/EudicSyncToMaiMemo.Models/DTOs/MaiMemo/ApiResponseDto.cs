using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo
{
    /// <summary>
    /// 墨墨背单词 API 返回结果
    /// </summary>
    public class ApiResponseDto
    {
        [JsonProperty("valid")]
        public int Valid { get; set; } = 0;

        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}
