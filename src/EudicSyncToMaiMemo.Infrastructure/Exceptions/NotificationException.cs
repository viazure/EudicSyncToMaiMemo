namespace EudicSyncToMaiMemo.Infrastructure.Exceptions
{
    /// <summary>
    /// 消息通知异常
    /// </summary>
    [Serializable]
    public class NotificationException : Exception
    {
        public NotificationException()
        {
        }

        public NotificationException(string? message) : base(message)
        {
        }

        public NotificationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}