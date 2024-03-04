namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    public interface IHttpHelper
    {
        Task<string> GetAsync(string uri, Dictionary<string, string>? headers = null);

        Task<string> PostAsync(string uri, string requestJson, Dictionary<string, string>? headers = null);

        Task<(string response, Dictionary<string, string> cookie)> PostFoRmAsync(string uri, FormUrlEncodedContent formData, Dictionary<string, string>? headers = null);
    }
}
