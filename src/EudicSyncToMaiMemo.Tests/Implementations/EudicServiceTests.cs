using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Models.DTOs.Eudic;
using EudicSyncToMaiMemo.Services.Implementations;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using Xunit;

namespace EudicSyncToMaiMemo.Tests.Implementations
{
    /// <summary>
    /// EudicService 分页拉取单元测试
    /// </summary>
    public class EudicServiceTests
    {
        [Fact]
        public async Task GetWords_WithMultiplePages_FetchesAllPages()
        {
            var httpHelper = new Mock<IHttpHelper>();
            var callCount = 0;

            httpHelper
                .Setup(h => h.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    callCount++;
                    return callCount switch
                    {
                        1 => SerializeWords(Enumerable.Range(1, 100).Select(i => $"word{i}")),
                        2 => SerializeWords(Enumerable.Range(101, 25).Select(i => $"word{i}")),
                        _ => SerializeWords([])
                    };
                });

            var service = CreateService(httpHelper.Object);
            var words = await service.GetWords("0");

            Assert.Equal(125, words.Count);
            Assert.Equal(2, callCount);
            Assert.Equal("word1", words[0]);
            Assert.Equal("word125", words[124]);
        }

        [Fact]
        public async Task GetWords_WithEmptyBook_ReturnsEmptyWithoutExtraPages()
        {
            var httpHelper = new Mock<IHttpHelper>();
            httpHelper
                .Setup(h => h.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>?>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(SerializeWords([]));

            var service = CreateService(httpHelper.Object);
            var words = await service.GetWords("0");

            Assert.Empty(words);
            httpHelper.Verify(
                h => h.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task GetWords_UsesCategoryIdQueryParameter()
        {
            var httpHelper = new Mock<IHttpHelper>();
            string? capturedUrl = null;

            httpHelper
                .Setup(h => h.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<Dictionary<string, string>?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, Dictionary<string, string>?, CancellationToken>((url, _, _) => capturedUrl = url)
                .ReturnsAsync(SerializeWords(["hello"]));

            var service = CreateService(httpHelper.Object);
            await service.GetWords("42");

            Assert.NotNull(capturedUrl);
            Assert.Contains("studylist/words?", capturedUrl);
            Assert.Contains("category_id=42", capturedUrl);
            Assert.Contains("page=0", capturedUrl);
            Assert.Contains("page_size=100", capturedUrl);
        }

        private static EudicService CreateService(IHttpHelper httpHelper)
        {
            var options = Options.Create(new EudicOptions { Authorization = "NIS test" });
            return new EudicService(options, httpHelper, NullLogger<EudicService>.Instance);
        }

        private static string SerializeWords(IEnumerable<string> words)
        {
            var dto = new ApiResponseDto<WordDto>
            {
                Data = words.Select(w => new WordDto { Word = w }).ToList()
            };
            return JsonConvert.SerializeObject(dto);
        }
    }
}
