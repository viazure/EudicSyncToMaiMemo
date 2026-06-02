using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Models.DTOs.MaiMemo.OpenApi;
using EudicSyncToMaiMemo.Services.Helpers;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Services.Implementations.MaiMemo
{
    /// <summary>
    /// 通过墨墨开放 API 同步云词库
    /// </summary>
    public sealed class MaiMemoOpenApiService(
        IOptions<MaiMemoOptions> maiMemoOptions,
        IHttpHelper httpHelper,
        INotificationService notificationService,
        ILogger<MaiMemoOpenApiService> logger) : IMaiMemoService
    {
        private const string ApiBaseUrl = "https://open.maimemo.com/open/api/v1/";
        private const int NotepadListPageSize = 100;

        /// <inheritdoc />
        public async Task SyncToMaimemoNotepad(
            string notepadId,
            List<string> eudicWords,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            string resolvedNotepadId = await ResolveNotepadIdAsync(notepadId, cancellationToken);

            var notepad = await GetNotepadAsync(resolvedNotepadId, cancellationToken);
            var existingLines = NotepadWordMergeHelper.SplitContentLines(notepad.Content);
            var (newWords, combinedLines) = NotepadWordMergeHelper.Merge(existingLines, eudicWords);

            if (newWords.Count > 0)
            {
                notepad.Content = NotepadWordMergeHelper.JoinContentLines(combinedLines);
                await SaveNotepadAsync(resolvedNotepadId, notepad, cancellationToken);
            }

            await SendNotificationAsync(newWords, cancellationToken);
        }

        private async Task<string> ResolveNotepadIdAsync(string configuredId, CancellationToken cancellationToken)
        {
            if (MaiMemoNotepadIdHelper.IsOpenApiNotepadId(configuredId))
            {
                return configuredId;
            }

            MaiMemoOptions options = maiMemoOptions.Value;
            string? title = options.DefaultNotepadTitle;
            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ConfigurationException(
                    "Open API 模式的 DefaultNotepadId 需为 np- 开头的词库 ID，请通过 GET /notepads 获取");
            }

            var notepads = await ListNotepadsAsync(cancellationToken);
            string? resolvedId = MaiMemoNotepadIdHelper.FindIdByTitle(notepads, title);
            if (resolvedId == null)
            {
                throw new ConfigurationException($"未找到标题为「{title}」的云词库，请检查 DefaultNotepadTitle");
            }

            logger.LogInformation(
                "通过标题 {Title} 解析 Open API 云词库 ID：{NotepadId}（配置的 DefaultNotepadId 为网页版数字 ID，已忽略）",
                title,
                resolvedId);

            return resolvedId;
        }

        private async Task<List<BriefNotepadDto>> ListNotepadsAsync(CancellationToken cancellationToken)
        {
            var notepads = new List<BriefNotepadDto>();

            for (int offset = 0; ; offset += NotepadListPageSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string url = $"{ApiBaseUrl}notepads?limit={NotepadListPageSize}&offset={offset}";
                string responseJson = await httpHelper.GetAsync(url, CreateAuthHeaders(), cancellationToken);
                var response = JsonHelper.JsonToObj<EnvelopeDto<NotepadListPayloadDto>>(responseJson);
                var page = response?.Data?.Notepads;

                if (page == null || page.Count == 0)
                {
                    break;
                }

                notepads.AddRange(page);

                if (page.Count < NotepadListPageSize)
                {
                    break;
                }
            }

            return notepads;
        }

        private async Task<NotepadDto> GetNotepadAsync(string notepadId, CancellationToken cancellationToken)
        {
            string url = BuildNotepadUrl(notepadId);
            string responseJson = await httpHelper.GetAsync(url, CreateAuthHeaders(), cancellationToken);
            var response = JsonHelper.JsonToObj<EnvelopeDto<NotepadPayloadDto>>(responseJson);

            if (response?.Data?.Notepad == null)
            {
                throw new InvalidOperationException(
                    $"获取墨墨云词库失败，响应解析为空。请确认 DefaultNotepadId 为 Open API 的 np- ID。原始响应：{responseJson}");
            }

            return response.Data.Notepad;
        }

        private async Task SaveNotepadAsync(
            string notepadId,
            NotepadDto notepad,
            CancellationToken cancellationToken)
        {
            string url = BuildNotepadUrl(notepadId);
            var request = new NotepadUpdateRequestDto
            {
                Notepad = new NotepadUpdateDto
                {
                    Status = notepad.Status,
                    Content = notepad.Content,
                    Title = notepad.Title,
                    Brief = notepad.Brief,
                    Tags = notepad.Tags ?? []
                }
            };
            string requestJson = JsonHelper.ObjToJson(request);
            string responseJson = await httpHelper.PostAsync(url, requestJson, CreateAuthHeaders(), cancellationToken);

            var response = JsonHelper.JsonToObj<EnvelopeDto<NotepadPayloadDto>>(responseJson);
            if (response?.Data?.Notepad == null)
            {
                throw new InvalidOperationException($"保存墨墨云词库失败：{responseJson}");
            }
        }

        private static string BuildNotepadUrl(string notepadId)
        {
            return $"{ApiBaseUrl}notepads/{Uri.EscapeDataString(notepadId)}";
        }

        private Dictionary<string, string> CreateAuthHeaders()
        {
            string? token = maiMemoOptions.Value.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new ConfigurationException("墨墨开放 API Token 为空");
            }

            return new Dictionary<string, string>
            {
                { "Authorization", $"Bearer {token}" },
                { "Accept", "application/json" }
            };
        }

        private async Task SendNotificationAsync(
            IReadOnlyList<string> words,
            CancellationToken cancellationToken)
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
