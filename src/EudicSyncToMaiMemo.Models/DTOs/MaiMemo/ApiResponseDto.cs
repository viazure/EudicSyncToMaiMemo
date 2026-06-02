using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo
{
    /// <summary>
    /// 墨墨网页 API 通用响应
    /// </summary>
    public class ApiResponseDto
    {
        /// <summary>
        /// 是否成功，1 表示有效
        /// </summary>
        [JsonProperty("valid")]
        public int Valid { get; set; }

        /// <summary>
        /// 错误描述，成功时通常为空
        /// </summary>
        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}
