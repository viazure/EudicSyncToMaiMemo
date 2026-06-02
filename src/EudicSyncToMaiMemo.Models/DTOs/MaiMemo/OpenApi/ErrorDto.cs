using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// 信封中的错误项
    /// </summary>
    public sealed class ErrorDto
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("code")]
        public string? Code { get; set; }

        /// <summary>
        /// 错误描述
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }
    }
}
