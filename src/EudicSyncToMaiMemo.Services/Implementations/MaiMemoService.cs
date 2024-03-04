using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.DTOs.MaiMemo;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 墨墨背单词服务实现
    /// </summary>
    public class MaiMemoService(IConfiguration configuration, IHttpHelper httpHelper, ILogger<MaiMemoService> logger) : IMaiMemoService
    {

        public async Task Test()
        {
            var foo = await GetCookies();

            logger.LogInformation("foo: {text}", string.Join(",", foo));
        }

        private async Task<Dictionary<string, string>> GetCookies()
        {
            string url = "https://www.maimemo.com/auth/login";

            var headers = new Dictionary<string, string>
            {
                { "authority", "www.maimemo.com" },
                { "accept", "application/json, text/javascript, */*; q=0.01"},
                { "accept-language", "zh-CN,zh;q=0.9"},
                { "origin", "https://www.maimemo.com"},
                { "referer", "https://www.maimemo.com/home/login" },
            };

            var formData = GetLoginForm();

            var (responseString, cookie) = await httpHelper.PostFoRmAsync(url, formData, headers);
            var response = JsonHelper.JsonToObj<ApiResponse>(responseString);

            const int ValidNumber = 1;
            if (response == null || response.Valid != ValidNumber)
            {
                throw new Exception($"墨墨背单词登录失败，原因：{response?.Error}。");
            }

            return cookie;
        }

        private FormUrlEncodedContent GetLoginForm()
        {
            string username = GetUsername();
            string password = GetPassword();

            var collection = new List<KeyValuePair<string, string>>
            {
                new("email", username),
                new("password", password)
            };
            var content = new FormUrlEncodedContent(collection);

            return content;
        }

        /// <summary>
        /// 获取配置中的墨墨背单词用户名
        /// </summary>
        /// <returns>Url Encode 后的用户名</returns>
        private string GetUsername()
        {
            string? username = configuration["MaiMemo:Username"];

            if (string.IsNullOrWhiteSpace(username))
            {
                throw new Exception("墨墨背单词用户名为空。");
            }

            return username;
        }

        /// <summary>
        /// 获取配置中的墨墨背单词用户名
        /// </summary>
        /// <returns>Url Encode 后的密码</returns>
        private string GetPassword()
        {
            string? password = configuration["MaiMemo:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("墨墨背单词密码为空。");
            }

            return password;
        }
    }
}
