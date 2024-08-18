using EudicSyncToMaiMemo.Models.DTOs.Notification;

namespace EudicSyncToMaiMemo.Services.Interfaces
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    public interface INotificationService
    {
        Task SendNotification(NotificationMessageDto messageDto);
    }
}
