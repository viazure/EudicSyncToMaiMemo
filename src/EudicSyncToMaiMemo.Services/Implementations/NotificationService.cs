using EudicSyncToMaiMemo.Infrastructure.Helpers;
using EudicSyncToMaiMemo.Models.DTOs.Notification;
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
        /// <param name="messageDto"></param>
        /// <returns></returns>
        public async Task SendNotification(NotificationMessageDto messageDto)
        {
            bool isEnabledNotification = configuration.GetValue<bool>("Notification:Enabled");

            if (!isEnabledNotification)
            {
                return;
            }

            string? templateUrl = configuration.GetValue<string>("Notification:Url");
            if (string.IsNullOrEmpty(templateUrl))
            {
                throw new Exception("通知失败，请检查通知服务 URL 配置。");
            }

            string url = ReplaceVariables(templateUrl, messageDto);
            string result = await httpHelper.GetAsync(url);

            logger.LogInformation("通知结果：{result}", result);
        }


        public static string ReplaceVariables(string templateUrl, NotificationMessageDto messageDto)
        {
            string result = templateUrl
                .Replace("{total}", messageDto.Total.ToString())
                .Replace("{content}", messageDto.Content);

            return result;
        }
    }
}
