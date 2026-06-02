using System.Text;

namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// 基于 IHttpClientFactory 的 HTTP 请求实现
    /// </summary>
    public class HttpHelper(IHttpClientFactory httpClientFactory) : IHttpHelper
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

        /// <inheritdoc />
        public async Task<string> GetAsync(
            string uri,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<string> PostAsync(
            string uri,
            string requestJson,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
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

            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<(string response, Dictionary<string, string> cookie)> PostFoRmAsync(
            string uri,
            FormUrlEncodedContent formData,
            Dictionary<string, string>? headers = null,
            CancellationToken cancellationToken = default)
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = new System.Net.CookieContainer()
            };

            using var httpClient = new HttpClient(handler);
            using var request = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = formData
            };

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            var response = await httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            var cookieCollection = handler.CookieContainer.GetCookies(new Uri(uri));
            var cookies = cookieCollection
                .Cast<System.Net.Cookie>()
                .ToDictionary(cookie => cookie.Name, cookie => cookie.Value);

            return (responseBody, cookies);
        }
    }
}
