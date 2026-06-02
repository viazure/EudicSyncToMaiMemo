namespace EudicSyncToMaiMemo.Models.Configuration
{
    /// <summary>
    /// 墨墨云词库同步接入方式
    /// </summary>
    public enum MaiMemoProvider
    {
        /// <summary>
        /// 墨墨开放 API，Bearer Token 认证
        /// </summary>
        OpenApi,

        /// <summary>
        /// 网页登录与表单保存，过渡方案
        /// </summary>
        Legacy
    }
}
