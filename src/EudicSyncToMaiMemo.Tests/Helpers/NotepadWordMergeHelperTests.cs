using EudicSyncToMaiMemo.Services.Helpers;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    /// <summary>
    /// NotepadWordMergeHelper 单元测试
    /// </summary>
    public class NotepadWordMergeHelperTests
    {
        [Fact]
        public void Merge_WithNewWords_ReturnsFilteredAndCombined()
        {
            var existing = new List<string> { "apple", "# chapter" };
            var eudic = new List<string> { "apple", "banana" };

            var (newWords, combined) = NotepadWordMergeHelper.Merge(existing, eudic);

            Assert.Single(newWords);
            Assert.Equal("banana", newWords[0]);
            Assert.Equal(3, combined.Count);
        }

        [Fact]
        public void Merge_WithNoNewWords_ReturnsEmptyNewWords()
        {
            var existing = new List<string> { "apple" };
            var eudic = new List<string> { "apple" };

            var (newWords, combined) = NotepadWordMergeHelper.Merge(existing, eudic);

            Assert.Empty(newWords);
            Assert.Single(combined);
        }

        [Fact]
        public void SplitContentLines_SplitsByNewline()
        {
            var lines = NotepadWordMergeHelper.SplitContentLines("apple\n# chapter\nbanana");

            Assert.Equal(3, lines.Count);
            Assert.Equal("# chapter", lines[1]);
        }
    }
}
