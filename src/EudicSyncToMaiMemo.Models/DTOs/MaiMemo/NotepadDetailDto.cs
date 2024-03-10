namespace EudicSyncToMaiMemo.Models.DTOs.MaiMemo
{
    /// <summary>
    /// 云词库详情
    /// </summary>
    public class NotepadDetailDto
    {
        /// <summary>
        /// 云词库ID
        /// </summary>
        public string NotepadId { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 简介
        /// </summary>
        public string Brief { get; set; } = string.Empty;

        /// <summary>
        /// 英文文本列表
        /// </summary>
        public List<string> ContentList { get; set; } = [];

        /// <summary>
        /// 是否为隐私词库
        /// </summary>
        public bool IsPrivacy { get; set; }

        /// <summary>
        /// 词库分类 ID 列表
        /// </summary>
        public List<string> Tags { get; set; } = [];
    }
}
