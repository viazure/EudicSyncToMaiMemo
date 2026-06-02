using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// GET/POST 单条云词本响应的 data 节点
    /// </summary>
    public sealed class NotepadPayloadDto
    {
        /// <summary>
        /// 云词本详情
        /// </summary>
        [JsonProperty("notepad")]
        public NotepadDto? Notepad { get; set; }
    }
}
