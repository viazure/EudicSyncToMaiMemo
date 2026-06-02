using Newtonsoft.Json;

namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi
{
    /// <summary>
    /// POST /notepads/{id} 更新请求体
    /// </summary>
    public sealed class NotepadUpdateRequestDto
    {
        /// <summary>
        /// 待提交的更新字段
        /// </summary>
        [JsonProperty("notepad")]
        public NotepadUpdateDto Notepad { get; set; } = new();
    }
}
