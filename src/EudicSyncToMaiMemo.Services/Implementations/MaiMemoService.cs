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
    public class MaiMemoService : IMaiMemoService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpHelper _httpHelper;
        private readonly ILogger<MaiMemoService> _logger;
        private readonly Dictionary<string, string> _headers;

        public MaiMemoService(IConfiguration configuration, IHttpHelper httpHelper, ILogger<MaiMemoService> logger)
        {
            _configuration = configuration;
            _httpHelper = httpHelper;
            _logger = logger;
            _headers = new Dictionary<string, string>
            {
                { "authority", "www.maimemo.com" },
                { "accept", "application/json, text/javascript, */*; q=0.01"},
                { "accept-language", "zh-CN,zh;q=0.9"},
                { "origin", "https://www.maimemo.com"},
                { "referer", "https://www.maimemo.com/home/login" },
            };
        }


        /// <summary>
        /// 获取 Cookie
        /// </summary>
        /// <returns>Cookie 键值对</returns>
        private async Task SetCookie()
        {
            string url = "https://www.maimemo.com/auth/login";

            var formData = GetLoginForm();

            var (responseString, cookie) = await _httpHelper.PostFoRmAsync(url, formData, _headers);
            var response = JsonHelper.JsonToObj<ApiResponse>(responseString);

            const int ValidNumber = 1;
            if (response == null || response.Valid != ValidNumber)
            {
                throw new Exception($"墨墨背单词登录失败，原因：{response?.Error}。");
            }

            if (cookie == null || cookie.Count == 0)
            {
                throw new Exception("获取 Cookie 失败，Cookie 内容为空。");
            }

            string cookieString = string.Join(";", cookie.Select(x => $"{x.Key}={x.Value}"));
            _headers.Add("cookie", cookieString);
        }

        /// <summary>
        /// 获取登录表单
        /// </summary>
        /// <returns></returns>
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
            string? username = _configuration["MaiMemo:Username"];

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
            string? password = _configuration["MaiMemo:Password"];

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("墨墨背单词密码为空。");
            }

            return password;
        }


        public async Task<List<string>> GetWords(string? notepadId = null)
        {
            await SetCookie();
            _logger.LogInformation("headers: {text}", string.Join("；", _headers.Select(x => $"{x.Key}={x.Value}")));

            string response = await GetNotepadDetailPage(notepadId);
            _logger.LogInformation("response: {text}", response);

            return [];
        }

        /// <summary>
        /// 获取云词库详情页面
        /// </summary>
        /// <param name="notepadId">云词库 ID</param>
        /// <returns>云词库详情页面网页源码</returns>
        private async Task<string> GetNotepadDetailPage(string? notepadId = null)
        {
            notepadId ??= _configuration.GetSection("MaiMemo:DefaultNotepadId").Value ?? "0";
            string url = $"https://www.maimemo.com/notepad/detail/{notepadId}";

            var response = await _httpHelper.GetAsync(url, _headers);

            return response;
        }
    }
}
