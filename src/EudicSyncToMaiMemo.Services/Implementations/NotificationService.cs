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
        public async Task SendNotification(string message, bool isSuccess = true)
        {
            if (!configuration.GetValue<bool>("Notification:Enabled"))
            {
                return;
            }

            string? templateUrl = configuration.GetValue<string>("Notification:Url");
            if (string.IsNullOrWhiteSpace(templateUrl))
            {
                logger.LogError("通知服务 URL 配置缺失。");
                throw new InvalidOperationException("通知服务 URL 配置缺失。");
            }


            string requestUrl = ReplaceVariables(templateUrl, message, isSuccess);
            if (!Uri.IsWellFormedUriString(requestUrl, UriKind.Absolute))
            {
                logger.LogError("请求 URL 格式不正确: {RequestUrl}", requestUrl);
                throw new InvalidOperationException("请求 URL 格式不正确。");
            }

            var headers = ParseHeaders(configuration.GetValue<string>("Notification:Headers"));
            string? requestBody = configuration.GetValue<string>("Notification:RequestBody");

            string result = string.IsNullOrEmpty(requestBody) ?
                await httpHelper.GetAsync(requestUrl, headers) :
                await SendPostRequest(requestBody, requestUrl, headers);

            logger.LogInformation("通知结果：{result}", result);
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
                throw new InvalidOperationException("通知失败，请求体不是正确的 JSON 格式。");
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
        public static string ReplaceVariables(string templateUrl, string message, bool isSuccess = true)
        {
            string result = templateUrl
                    .Replace("{result}", isSuccess ? "同步成功" : "同步失败")
                    .Replace("{content}", message);

            return result;
        }
    }
}
