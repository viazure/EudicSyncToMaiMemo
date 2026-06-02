using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.Configuration;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 同步结果 Webhook 通知
    /// </summary>
    public sealed class NotificationService(
        IOptions<NotificationOptions> notificationOptions,
        IHttpHelper httpHelper,
        ILogger<NotificationService> logger) : INotificationService
    {
        /// <inheritdoc />
        public async Task SendNotification(string message, CancellationToken cancellationToken = default)
        {
            NotificationOptions options = notificationOptions.Value;

            if (!options.Enabled)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(options.Url))
            {
                throw new NotificationException("未配置通知 URL");
            }

            string requestUrl = ReplaceVariables(options.Url, message);
            var headers = ParseHeaders(options.Headers);

            try
            {
                string result = string.IsNullOrEmpty(options.RequestBody)
                    ? await httpHelper.GetAsync(requestUrl, headers, cancellationToken)
                    : await SendPostRequestAsync(options.RequestBody, requestUrl, headers, cancellationToken);

                logger.LogInformation("通知成功：{Result}", result);
            }
            catch (Exception ex)
            {
                throw new NotificationException($"通知失败。通知 URL：{requestUrl}", ex);
            }
        }

        private static Dictionary<string, string>? ParseHeaders(string headersStr)
        {
            return string.IsNullOrEmpty(headersStr)
                ? null
                : headersStr.Split(';')
                    .Select(x => x.Split('='))
                    .ToDictionary(x => x[0], x => x[1]);
        }

        private async Task<string> SendPostRequestAsync(
            string requestBody,
            string requestUrl,
            Dictionary<string, string>? headers,
            CancellationToken cancellationToken)
        {
            if (!JsonHelper.IsValidJson(requestBody))
            {
                throw new NotificationException("请求体不是正确的 JSON 格式");
            }

            return await httpHelper.PostAsync(requestUrl, requestBody, headers, cancellationToken);
        }

        private static string ReplaceVariables(string templateUrl, string message)
        {
            return templateUrl.Replace("{content}", message);
        }
    }
}
