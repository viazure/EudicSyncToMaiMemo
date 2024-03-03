using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo
{
    public class ApiResponse
    {
        [JsonProperty("valid")]
        public int Valid { get; set; } = 0;

        [JsonProperty("error")]
        public string? Error { get; set; }
    }
}
