using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.Eudic
{
    /// <summary>
    /// 欧路词典 Open API 通用响应包装
    /// </summary>
    /// <typeparam name="T">data 数组元素类型</typeparam>
    public class ApiResponseDto<T>
    {
        /// <summary>
        /// 业务数据列表，失败或空结果时可能为 null
        /// </summary>
        [JsonProperty("data")]
        public List<T>? Data { get; set; }

        /// <summary>
        /// 接口提示信息
        /// </summary>
        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;
    }
}
