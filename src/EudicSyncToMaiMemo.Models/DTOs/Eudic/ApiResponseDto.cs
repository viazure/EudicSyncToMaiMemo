using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路词典 API 返回结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponseDto<T>
    {
        [JsonProperty("data")]
        public List<T>? Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
