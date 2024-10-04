namespace EudicSyncToMaiMemo.Infrastructure.Exceptions
{
    /// <summary>
    /// 配置信息异常
    /// </summary>
    [Serializable]
    public class ConfigurationException : Exception
    {
        public ConfigurationException()
        {
        }

        public ConfigurationException(string? message) : base(message)
        {
        }

        public ConfigurationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}