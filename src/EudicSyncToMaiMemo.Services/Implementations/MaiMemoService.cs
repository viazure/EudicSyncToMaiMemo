using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.DTOs.MaiMemo;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

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
        private static readonly string[] separator = ["\r\n", "\n"];

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
        /// 同步到墨墨背单词云词库
        /// </summary>
        /// <param name="notepadId">墨墨背单词云词库 ID</param>
        /// <param name="eudicWords">待同步的欧路词典单词列表</param>
        /// <returns></returns>
        public async Task<bool> SyncToMaimemoNotepad(string notepadId, IEnumerable<string> eudicWords)
        {
            // STEP 1: 登录获取 Cookie，并设置请求头
            await SetCookie();

            // STEP 2: 获取云词库详情页面
            string notepadDetailHtml = await GetNotepadDetail(notepadId);

            // STEP 3: 解析云词库详情页面
            var notepadDetail = await ParseNotepadDetail(notepadId, notepadDetailHtml);

            // Todo: STEP 4: 过滤出需要同步的单词并组织新的云词库保存内容
            var saveParam = CreateSaveParam(notepadDetail, eudicWords);

            // Todo: STEP 5: 保存到云词库
            await SaveNotepad(saveParam);

            return true;
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


        /// <summary>
        /// 获取云词库详情页面
        /// </summary>
        /// <param name="notepadId">云词库 ID</param>
        /// <returns>云词库详情页面网页源码</returns>
        private async Task<string> GetNotepadDetail(string notepadId)
        {
            string url = $"https://www.maimemo.com/notepad/detail/{notepadId}";

            var responseHtml = await _httpHelper.GetAsync(url, _headers);

            return responseHtml;
        }

        /// <summary>
        /// 解析云词库详情页面
        /// </summary>
        /// <param name="notepadId">云词库 ID</param>
        /// <param name="html">云词库详情页面 Html 文本</param>
        /// <returns>云词库详情 DTO</returns>
        private static async Task<NotepadDetailDto> ParseNotepadDetail(string notepadId, string html)
        {
            using var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(html));

            return new NotepadDetailDto
            {
                NotepadId = notepadId,
                Title = ParseTitle(document),
                Brief = ParseBrief(document),
                ContentList = ParseContentList(document),
                IsPrivacy = ParsePrivacy(document),
                Tags = ParseTags(document)
            };
        }

        private static string ParseTitle(IDocument document)
        {
            return document.QuerySelector<IHtmlInputElement>("#title")?.Value ?? string.Empty;
        }

        private static string ParseBrief(IDocument document)
        {
            return document.QuerySelector<IHtmlInputElement>("#brief")?.Value ?? string.Empty;
        }

        private static List<string> ParseContentList(IDocument document)
        {
            var content = document.QuerySelector<IHtmlTextAreaElement>("#content")?.TextContent;
            return content?.Split(separator, StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];
        }

        private static bool ParsePrivacy(IDocument document)
        {
            const string IsPrivacyFlag = "1";
            var notepadPrivacyElements = document.QuerySelectorAll("#notepadPrivacy a.active");
            return notepadPrivacyElements.Any(e => e.GetAttribute("data-private") == IsPrivacyFlag);
        }

        private static List<string> ParseTags(IDocument document)
        {
            var notepadTagElements = document.QuerySelectorAll("#notepadTags a.active");
            return notepadTagElements.Select(e => e.GetAttribute("data-tag") ?? string.Empty).ToList();
        }


        /// <summary>
        /// 组织新的云词库保存内容
        /// </summary>
        /// <param name="originalNotepadDetail">原云词库明细</param>
        /// <param name="eudicWords"></param>
        /// <returns></returns>
        private FormUrlEncodedContent CreateSaveParam(NotepadDetailDto originalNotepadDetail, IEnumerable<string> eudicWords)
        {
            var contentToSync = FilterWordToSync(originalNotepadDetail.ContentList, eudicWords);

            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", originalNotepadDetail.NotepadId.ToString()),
                new KeyValuePair<string, string>("title", originalNotepadDetail.Title),
                new KeyValuePair<string, string>("brief", originalNotepadDetail.Brief),
                new KeyValuePair<string, string>("content", contentToSync),
                new KeyValuePair<string, string>("is_private", originalNotepadDetail.IsPrivacy.ToString().ToLower())
            };

            if (originalNotepadDetail.Tags.Count > 0)
            {
                foreach (var tag in originalNotepadDetail.Tags)
                {
                    formData.Add(new KeyValuePair<string, string>("tag[]", tag));
                }
            }

            return new FormUrlEncodedContent(formData);
        }

        /// <summary>
        /// 过滤出需要同步的单词
        /// </summary>
        /// <param name="syncedWords">已同步的单词</param>
        /// <param name="eudicWords">欧路词典的单词</param>
        /// <returns></returns>
        private string FilterWordToSync(
            IEnumerable<string> syncedWords, IEnumerable<string> eudicWords)
        {
            // 将欧路词典生词本多于墨墨背单词的云词库的单词提取出来，加入到墨墨背单词的云词库中
            var eudicWordsToSync = eudicWords.Except(syncedWords);
            var wordsToSync = syncedWords.Concat(eudicWordsToSync);

            var contentToSync = string.Join("\n", wordsToSync);

            _logger.LogInformation("新增单词数量 {count} 条。", eudicWordsToSync.Count());
            if (eudicWordsToSync.Any())
            {
                _logger.LogInformation("新增单词内容：{content} …", string.Join(", ", eudicWordsToSync.Take(10)));
            }

            return contentToSync;
        }


        /// <summary>
        /// 保存墨墨云词库
        /// </summary>
        /// <param name="formData">新云词库表单数据</param>
        /// <returns></returns>
        private async Task SaveNotepad(FormUrlEncodedContent formData)
        {
            string url = "https://www.maimemo.com/notepad/save";

            var (responseString, _) = await _httpHelper.PostFoRmAsync(url, formData, _headers);
            var response = JsonHelper.JsonToObj<ApiResponse>(responseString);

            const int ValidNumber = 1;
            if (response == null || response.Valid != ValidNumber)
            {
                throw new Exception($"同步失败，原因：{response?.Error}。");
            }
        }
    }
}
