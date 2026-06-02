using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Models.DTOs.MaiMemo;
using EudicSyncToMaiMemo.Services.Helpers;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Services.Implementations.MaiMemo
{
    /// <summary>
    /// 通过墨墨网页登录与表单保存同步云词库，Legacy 过渡方案
    /// </summary>
    public sealed class MaiMemoLegacyWebService(
        IOptions<MaiMemoOptions> maiMemoOptions,
        IHttpHelper httpHelper,
        INotificationService notificationService,
        ILogger<MaiMemoLegacyWebService> logger) : IMaiMemoService
    {
        private readonly Dictionary<string, string> _headers = new()
        {
            { "authority", "www.maimemo.com" },
            { "accept", "application/json, text/javascript, */*; q=0.01" },
            { "accept-language", "zh-CN,zh;q=0.9" },
            { "origin", "https://www.maimemo.com" },
        };

        private static readonly string[] Separator = ["\r\n", "\n"];

        /// <inheritdoc />
        public async Task SyncToMaimemoNotepad(
            string notepadId,
            List<string> eudicWords,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await SetCookieAsync(cancellationToken);

            string notepadDetailHtml = await GetNotepadDetailAsync(notepadId, cancellationToken);
            var notepadDetail = await ParseNotepadDetailAsync(notepadId, notepadDetailHtml);
            var (newWords, combinedLines) = NotepadWordMergeHelper.Merge(notepadDetail.ContentList, eudicWords);

            if (newWords.Count > 0)
            {
                var saveParam = CreateSaveParam(notepadDetail, combinedLines);
                await SaveNotepadAsync(saveParam, cancellationToken);
            }

            await SendNotificationAsync(newWords, cancellationToken);
        }

        private async Task SetCookieAsync(CancellationToken cancellationToken)
        {
            const string url = "https://www.maimemo.com/auth/login";
            var formData = GetLoginForm();
            var (responseString, cookie) = await httpHelper.PostFoRmAsync(url, formData, _headers, cancellationToken);
            var response = JsonHelper.JsonToObj<ApiResponseDto>(responseString);

            const int validNumber = 1;
            if (response == null || response.Valid != validNumber)
            {
                throw new InvalidOperationException($"墨墨背单词登录失败，原因：{response?.Error}");
            }

            if (cookie == null || cookie.Count == 0)
            {
                throw new InvalidOperationException("获取 Cookie 失败，Cookie 内容为空");
            }

            string cookieString = string.Join(";", cookie.Select(x => $"{x.Key}={x.Value}"));
            _headers["cookie"] = cookieString;
        }

        private FormUrlEncodedContent GetLoginForm()
        {
            MaiMemoOptions options = maiMemoOptions.Value;
            string username = options.Username
                ?? throw new ConfigurationException("墨墨背单词用户名为空");
            string password = options.Password
                ?? throw new ConfigurationException("墨墨背单词密码为空");

            return new FormUrlEncodedContent(
            [
                new KeyValuePair<string, string>("email", username),
                new KeyValuePair<string, string>("password", password)
            ]);
        }

        private async Task<string> GetNotepadDetailAsync(string notepadId, CancellationToken cancellationToken)
        {
            string url = $"https://www.maimemo.com/notepad/detail/{notepadId}";
            return await httpHelper.GetAsync(url, _headers, cancellationToken);
        }

        private static async Task<NotepadDetailDto> ParseNotepadDetailAsync(string notepadId, string html)
        {
            using var context = BrowsingContext.New(AngleSharp.Configuration.Default);
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
            return content?.Split(Separator, StringSplitOptions.RemoveEmptyEntries).ToList() ?? [];
        }

        private static bool ParsePrivacy(IDocument document)
        {
            const string isPrivacyFlag = "1";
            var notepadPrivacyElements = document.QuerySelectorAll("#notepadPrivacy a.active");
            return notepadPrivacyElements.Any(e => e.GetAttribute("data-private") == isPrivacyFlag);
        }

        private static List<string> ParseTags(IDocument document)
        {
            var notepadTagElements = document.QuerySelectorAll("#notepadTags a.active");
            return notepadTagElements.Select(e => e.GetAttribute("data-tag") ?? string.Empty).ToList();
        }

        private static FormUrlEncodedContent CreateSaveParam(
            NotepadDetailDto originalNotepadDetail,
            IReadOnlyList<string> combinedLines)
        {
            string wordsToSyncStr = NotepadWordMergeHelper.JoinContentLines(combinedLines);

            var formData = new List<KeyValuePair<string, string>>
            {
                new("id", originalNotepadDetail.NotepadId),
                new("title", originalNotepadDetail.Title),
                new("brief", originalNotepadDetail.Brief),
                new("content", wordsToSyncStr),
                new("is_private", originalNotepadDetail.IsPrivacy.ToString().ToLowerInvariant())
            };

            foreach (var tag in originalNotepadDetail.Tags)
            {
                formData.Add(new KeyValuePair<string, string>("tag[]", tag));
            }

            return new FormUrlEncodedContent(formData);
        }

        private async Task SaveNotepadAsync(FormUrlEncodedContent formData, CancellationToken cancellationToken)
        {
            const string url = "https://www.maimemo.com/notepad/save";
            var (responseString, _) = await httpHelper.PostFoRmAsync(url, formData, _headers, cancellationToken);
            var response = JsonHelper.JsonToObj<ApiResponseDto>(responseString);

            const int validNumber = 1;
            if (response is not { Valid: validNumber })
            {
                throw new InvalidOperationException(
                    $"保存墨墨云词库失败：{response?.Error}。原始响应内容：{responseString}");
            }
        }

        private async Task SendNotificationAsync(IReadOnlyList<string> words, CancellationToken cancellationToken)
        {
            if (words.Count > 0)
            {
                string content = string.Join(", ", words);
                logger.LogInformation("新增单词数量 {Total} 条，内容：{Content}", words.Count, content);
                await notificationService.SendNotification(content, cancellationToken);
            }
            else
            {
                logger.LogInformation("新增单词数量 0 条");
                await notificationService.SendNotification("没有新增单词", cancellationToken);
            }
        }
    }
}
