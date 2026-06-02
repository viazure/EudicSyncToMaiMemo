using EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi;
using EudicSyncToMaiMemo.Services.Helpers;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Helpers
{
    /// <summary>
    /// MaiMemoNotepadIdHelper 单元测试
    /// </summary>
    public class MaiMemoNotepadIdHelperTests
    {
        [Theory]
        [InlineData("np-abc123", true)]
        [InlineData("1234567", false)]
        [InlineData("0", false)]
        public void IsOpenApiNotepadId_DetectsPrefix(string id, bool expected)
        {
            Assert.Equal(expected, MaiMemoNotepadIdHelper.IsOpenApiNotepadId(id));
        }

        [Fact]
        public void FindIdByTitle_WithUniqueMatch_ReturnsId()
        {
            var notepads = new List<BriefNotepadDto>
            {
                new() { Id = "np-aaa", Title = "生词" },
                new() { Id = "np-bbb", Title = "其他" }
            };

            string? id = MaiMemoNotepadIdHelper.FindIdByTitle(notepads, "生词");

            Assert.Equal("np-aaa", id);
        }

        [Fact]
        public void FindIdByTitle_WithDuplicateTitle_Throws()
        {
            var notepads = new List<BriefNotepadDto>
            {
                new() { Id = "np-aaa", Title = "生词" },
                new() { Id = "np-bbb", Title = "生词" }
            };

            Assert.Throws<InvalidOperationException>(() =>
                MaiMemoNotepadIdHelper.FindIdByTitle(notepads, "生词"));
        }
    }
}
