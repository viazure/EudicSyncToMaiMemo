using Microsoft.Extensions.Logging;
using System.Text;

namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// HTTP请求帮助类
    /// </summary>
    public class HttpHelper
    {
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;


        public HttpHelper(ILogger<HttpHelper> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Http Post with custom headers.
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <param name="data">请求数据</param>
        /// <param name="headers">请求头</param>
        /// <returns>Response content as string</returns>
        public async Task<string> HttpPostAsync(string requestUri, string data, Dictionary<string, string>? headers = null)
        {
            try
            {
                var content = new StringContent(data, Encoding.UTF8, "application/json");

                using var requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
                requestMessage.Content = content;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        requestMessage.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(requestMessage);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Http 请求失败，{response.StatusCode}");
                    return string.Empty;
                }

                string result = await response.Content.ReadAsStringAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "接口请求失败。");
            }

            return string.Empty;
        }
    }
}
