using EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi;

namespace EudicSyncToMaiMemo.Services.Helpers
{
    /// <summary>
    /// 墨墨 Open API 与网页版云词库 ID 识别与匹配
    /// </summary>
    public static class MaiMemoNotepadIdHelper
    {
        /// <summary>
        /// Open API 云词库 ID 前缀
        /// </summary>
        public const string OpenApiIdPrefix = "np-";

        /// <summary>
        /// 判断是否为 Open API 云词库 ID
        /// </summary>
        /// <param name="notepadId">配置或 API 返回的 ID</param>
        public static bool IsOpenApiNotepadId(string notepadId)
        {
            return notepadId.StartsWith(OpenApiIdPrefix, StringComparison.Ordinal);
        }

        /// <summary>
        /// 按标题在列表中查找唯一匹配的 Open API ID
        /// </summary>
        /// <param name="notepads">云词库摘要列表</param>
        /// <param name="title">待匹配标题，精确匹配</param>
        public static string? FindIdByTitle(
            IReadOnlyList<BriefNotepadDto> notepads,
            string title)
        {
            var matches = notepads
                .Where(n => string.Equals(n.Title, title, StringComparison.Ordinal))
                .ToList();

            return matches.Count switch
            {
                1 => matches[0].Id,
                > 1 => throw new InvalidOperationException($"存在多个标题为「{title}」的云词库，请改用 np- 开头的 DefaultNotepadId"),
                _ => null
            };
        }
    }
}
