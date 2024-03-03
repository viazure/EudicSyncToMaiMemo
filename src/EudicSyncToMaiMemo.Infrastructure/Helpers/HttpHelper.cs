using Microsoft.Extensions.Logging;
using System.Text;

namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// HTTP 请求帮助类
    /// </summary>
    public class HttpHelper : IHttpHelper
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpHelper> _logger;

        public HttpHelper(IHttpClientFactory httpClientFactory, ILogger<HttpHelper> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _logger = logger;
        }

        /// <summary>
        /// 发送 GET 请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
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

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("HTTP request failed:{StatusCode}", response.StatusCode);
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Interface request failed.");
            }

            return string.Empty;
        }

        /// <summary>
        /// 发送 JSON 请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestJson"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
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

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("HTTP request failed:{StatusCode}", response.StatusCode);
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Interface request failed.");
            }

            return string.Empty;
        }

        /// <summary>
        /// 发送文本请求
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="text"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public async Task<string> PostPlainTextAsync(string uri, string text, Dictionary<string, string>? headers = null)
        {
            try
            {
                var content = new StringContent(text, Encoding.UTF8, "text/plain");
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

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("HTTP request failed:{StatusCode}", response.StatusCode);
                    return string.Empty;
                }

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{Message}", ex.Message);
            }

            return string.Empty;
        }
    }
}
