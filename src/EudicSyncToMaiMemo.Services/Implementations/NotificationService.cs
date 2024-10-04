using EudicSyncToMaiMemo.Infrastructure.Exceptions;
using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EudicSyncToMaiMemo.Services.Implementations
{
    /// <summary>
    /// 通知服务实现
    /// </summary>
    public class NotificationService(
        IConfiguration configuration,
        IHttpHelper httpHelper,
        ILogger<NotificationService> logger) : INotificationService
    {
        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="isSuccess">同步是否成功</param>
        /// <returns></returns>
        public async Task SendNotification(string message)
        {
            if (!configuration.GetValue<bool>("Notification:Enabled"))
            {
                return;
            }

            string? templateUrl = configuration.GetValue<string>("Notification:Url");
            if (string.IsNullOrWhiteSpace(templateUrl))
            {
                throw new NotificationException("未配置通知 URL。");
            }

            string requestUrl = ReplaceVariables(templateUrl, message);
            if (!Uri.IsWellFormedUriString(requestUrl, UriKind.Absolute))
            {
                throw new NotificationException("配置的通知 URL 格式不正确。");
            }

            var headers = ParseHeaders(configuration.GetValue<string>("Notification:Headers"));
            string? requestBody = configuration.GetValue<string>("Notification:RequestBody");

            try
            {
                string result = string.IsNullOrEmpty(requestBody)
                    ? await httpHelper.GetAsync(requestUrl, headers) : await SendPostRequest(requestBody, requestUrl, headers);

                logger.LogInformation("通知成功：{result}", result);
            }
            catch (Exception ex)
            {
                throw new NotificationException("通知失败。", ex);
            }
        }

        /// <summary>
        /// 解析 Headers 配置
        /// </summary>
        /// <param name="headersStr">Headers 配置字符串，格式为 "key1=value1;key2=value2;..."</param>
        /// <returns>Headers 字典</returns>
        private Dictionary<string, string>? ParseHeaders(string? headersStr)
        {
            return string.IsNullOrEmpty(headersStr) ? null : headersStr.Split(';')
                .Select(x => x.Split('='))
                .ToDictionary(x => x[0], x => x[1]);
        }

        /// <summary>
        /// 发送 POST 请求
        /// </summary>
        /// <param name="requestBody">请求体</param>
        /// <param name="requestUrl">请求 URL</param>
        /// <param name="headers">请求头</param>
        /// <returns>响应内容</returns>
        private async Task<string> SendPostRequest(string requestBody, string requestUrl, Dictionary<string, string>? headers)
        {
            if (!JsonHelper.IsValidJson(requestBody))
            {
                throw new NotificationException("请求体不是正确的 JSON 格式。");
            }

            return await httpHelper.PostAsync(requestUrl, requestBody, headers);
        }


        /// <summary>
        /// 替换 URL 模板变量
        /// </summary>
        /// <param name="templateUrl">message 模板 URL</param>
        /// <param name="message">消息内容</param>
        /// <param name="isSuccess">同步是否成功</param>
        /// <returns>替换后的 URL 字符串</returns>
        public static string ReplaceVariables(string templateUrl, string message)
        {
            string result = templateUrl.Replace("{content}", message);

            return result;
        }
    }
}
