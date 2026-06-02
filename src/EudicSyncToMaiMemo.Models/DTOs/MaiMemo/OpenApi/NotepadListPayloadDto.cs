using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// GET 云词本列表响应的 data 节点
    /// </summary>
    public sealed class NotepadListPayloadDto
    {
        /// <summary>
        /// 云词本摘要列表
        /// </summary>
        [JsonProperty("notepads")]
        public List<BriefNotepadDto>? Notepads { get; set; }
    }
}
