namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// 对外部 HTTP API 的 GET/POST 封装
    /// </summary>
    public interface IHttpHelper
    {
        /// <summary>
        /// 发送 GET 请求并返回响应正文
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="headers">可选请求头</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<string> GetAsync(
            string uri,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送 JSON POST 请求并返回响应正文
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="requestJson">JSON 请求体</param>
        /// <param name="headers">可选请求头</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<string> PostAsync(
            string uri,
            string requestJson,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// 发送表单 POST 请求并返回响应正文与 Cookie
        /// </summary>
        /// <param name="uri">请求地址</param>
        /// <param name="formData">表单内容</param>
        /// <param name="headers">可选请求头</param>
        /// <param name="cancellationToken">取消令牌</param>
        Task<(string response, Dictionary<string, string> cookie)> PostFoRmAsync(
            string uri,
            FormUrlEncodedContent formData,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default);
    }
}
