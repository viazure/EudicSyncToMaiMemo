using Microsoft.Extensions.Logging;
using System.Text;

namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// HTTP 请求帮助类
    /// </summary>
    public class HttpHelper(IHttpClientFactory httpClientFactory, ILogger<HttpHelper> logger) : IHttpHelper
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <returns>响应消息体</returns>
        public async Task<string> GetAsync(string uri, Dictionary<string, string>? headers = null)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, uri);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GetAsync failed：{Message}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// 发送 JSON 请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestJson"></param>
        /// <param name="headers"></param>
        /// <returns>响应消息体</returns>
        public async Task<string> PostAsync(string uri, string requestJson, Dictionary<string, string>? headers = null)
        {
            try
            {
                var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
                using var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = content
                };

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PostAsync failed：{Message}", ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// 发送文本请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="text"></param>
        /// <param name="headers"></param>
        /// <returns>响应消息体，Cookie</returns>
        public async Task<(string response, Dictionary<string, string> cookie)> PostPlainTextAsync(
          string uri, string text, Dictionary<string, string>? headers = null)
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    CookieContainer = new System.Net.CookieContainer()
                };

                using var httpClient = new HttpClient(handler);
                var content = new StringContent(text, Encoding.UTF8, "application/x-www-form-urlencoded");
                using var request = new HttpRequestMessage(HttpMethod.Post, uri)
                {
                    Content = content
                };

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        request.Headers.Add(header.Key, header.Value);
                    }
                }

                var response = await httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Get cookies from the cookie container
                var cookieCollection = handler.CookieContainer.GetCookies(new Uri(uri));
                var cookies = cookieCollection
                  .Cast<System.Net.Cookie>()
                  .ToDictionary(cookie => cookie.Name, cookie => cookie.Value);

                return (responseBody, cookies);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PostPlainTextAsync failed：{Message}", ex.Message);
            }

            return (string.Empty, []);
        }
    }
}
