using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路词典接口响应
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        [JsonProperty("data")]
        public List<T>? Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
