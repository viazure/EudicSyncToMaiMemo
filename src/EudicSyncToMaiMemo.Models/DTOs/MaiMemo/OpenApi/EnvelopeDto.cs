using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// 墨墨开放 API 统一响应信封
    /// </summary>
    /// <typeparam name="TData">data 节点类型</typeparam>
    public sealed class EnvelopeDto<TData>
    {
        /// <summary>
        /// 错误列表，成功时通常为空数组
        /// </summary>
        [JsonProperty("errors")]
        public List<ErrorDto>? Errors { get; set; }

        /// <summary>
        /// 业务数据
        /// </summary>
        [JsonProperty("data")]
        public TData? Data { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
