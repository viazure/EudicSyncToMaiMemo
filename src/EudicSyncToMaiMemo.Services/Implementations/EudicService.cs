using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.DTOs.Eudic;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;


namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 欧路词典服务实现
    /// </summary>
    public class EudicService(IConfiguration configuration, IHttpHelper httpHelper) : IEudicService
    {
        private const string ApiEndpoint = "https://api.frdic.com/api/open/v1/";

        /// <summary>
        /// 获取所有单词本
        /// </summary>
        /// <returns></returns>
        public async Task<List<BookDto>> GetAllBooks()
        {
            string url = $"{ApiEndpoint}studylist/category?language=en";
            var headers = new Dictionary<string, string>
            {
                { "Authorization", GetAuthorization() }
            };
            string result = await httpHelper.GetAsync(url, headers);

            return JsonHelper.JsonToObj<ApiResponseDto<BookDto>>(result)?.Data ?? [];
        }

        /// <summary>
        /// 获取单词
        /// </summary>
        /// <param name="bookId">单词本 ID</param>
        /// <returns></returns>
        public async Task<List<string>> GetWords(string bookId)
        {
            string url = $"{ApiEndpoint}studylist/words/{bookId}?language=en";

            var headers = new Dictionary<string, string>
            {
                { "Authorization", GetAuthorization() }
            };
            string responseString = await httpHelper.GetAsync(url, headers);

            var eudicWords = JsonHelper.JsonToObj<ApiResponseDto<WordDto>>(responseString)?.Data;

            if (eudicWords == null)
            {
                return [];
            }

            return eudicWords.Select(x => x.Word).ToList();
        }

        /// <summary>
        /// 获取欧路词典授权
        /// </summary>
        /// <returns></returns>
        private string GetAuthorization()
        {
            string? authorization = configuration["Eudic:Authorization"];

            if (string.IsNullOrWhiteSpace(authorization))
            {
                throw new InvalidOperationException("未设置欧路词典授权信息（Authorization）。");
            }

            return authorization;
        }
    }
}
