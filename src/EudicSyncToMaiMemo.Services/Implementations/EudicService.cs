using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Models.DTOs.Eudic;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 欧路词典 Open API 生词本读取
    /// </summary>
    public sealed class EudicService(
        IOptions<EudicOptions> eudicOptions,
        IHttpHelper httpHelper,
        ILogger<EudicService> logger) : IEudicService
    {
        private const string ApiEndpoint = "https://api.frdic.com/api/open/v1/";
        private const int MaxPageSize = 100;
        private const int MaxPageIndex = 50;

        /// <inheritdoc />
        public async Task<List<BookDto>> GetAllBooks(CancellationToken cancellationToken = default)
        {
            string url = $"{ApiEndpoint}studylist/category?language=en";
            var headers = CreateAuthHeaders();
            string result = await httpHelper.GetAsync(url, headers, cancellationToken);

            return JsonHelper.JsonToObj<ApiResponseDto<BookDto>>(result)?.Data ?? [];
        }

        /// <inheritdoc />
        ///
        /// <remarks>
        /// 每页 page_size=100，page 从 0 递增，最大 50
        /// </remarks>
        public async Task<List<string>> GetWords(string bookId, CancellationToken cancellationToken = default)
        {
            var headers = CreateAuthHeaders();
            var words = new List<string>();

            for (int page = 0; page <= MaxPageIndex; page++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string url =
                    $"{ApiEndpoint}studylist/words?language=en&category_id={Uri.EscapeDataString(bookId)}&page={page}&page_size={MaxPageSize}";

                string responseString = await httpHelper.GetAsync(url, headers, cancellationToken);
                var pageWords = JsonHelper.JsonToObj<ApiResponseDto<WordDto>>(responseString)?.Data;

                if (pageWords == null || pageWords.Count == 0)
                {
                    break;
                }

                words.AddRange(pageWords.Select(x => x.Word));
                logger.LogDebug("欧路生词本 {BookId} 第 {Page} 页拉取 {Count} 个单词", bookId, page, pageWords.Count);

                if (pageWords.Count < MaxPageSize)
                {
                    break;
                }
            }

            logger.LogInformation("欧路生词本 {BookId} 共拉取 {Count} 个单词", bookId, words.Count);
            return words;
        }

        private Dictionary<string, string> CreateAuthHeaders()
        {
            return new Dictionary<string, string>
            {
                { "Authorization", GetAuthorization() }
            };
        }

        private string GetAuthorization()
        {
            string authorization = eudicOptions.Value.Authorization;

            if (string.IsNullOrWhiteSpace(authorization))
            {
                throw new ConfigurationException("未设置欧路词典授权信息（Authorization）");
            }

            return authorization;
        }
    }
}
