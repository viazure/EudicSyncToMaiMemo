namespace EudicSyncToMaiMemo.Services.Helpers
{
    /// <summary>
    /// 云词库内容与欧路词表增量合并
    /// </summary>
    public static class NotepadWordMergeHelper
    {
        private static readonly string[] LineSeparators = ["\r\n", "\n"];

        /// <summary>
        /// 合并已有行与欧路词表，返回新增词与合并后的行列表
        /// </summary>
        /// <param name="existingLines">云词库现有内容行，含章节行</param>
        /// <param name="eudicWords">欧路生词本单词</param>
        public static (IReadOnlyList<string> NewWords, IReadOnlyList<string> CombinedLines) Merge(
            IReadOnlyList<string> existingLines,
            IReadOnlyList<string> eudicWords)
        {
            var filteredWords = eudicWords.Except(existingLines).ToList();
            var combinedLines = existingLines.Concat(filteredWords).ToList();
            return (filteredWords, combinedLines);
        }

        /// <summary>
        /// 将云词库 content 文本拆分为行
        /// </summary>
        /// <param name="content">云词库原始 content</param>
        public static List<string> SplitContentLines(string content)
        {
            return content.Split(LineSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// 将行列表还原为 content 文本
        /// </summary>
        /// <param name="lines">合并后的行列表</param>
        public static string JoinContentLines(IReadOnlyList<string> lines)
        {
            return string.Join('\n', lines);
        }
    }
}
