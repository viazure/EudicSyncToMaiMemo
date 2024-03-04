using Newtonsoft.Json;
namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// JSON 帮助类
    /// </summary>
    public class JsonHelper
    {

        public static string ObjToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        public static T? JsonToObj<T>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}
