using Newtonsoft.Json;
namespace EudicSyncToMaiMemo.Infrastructure.Helpers
{
    /// <summary>
    /// JSON 帮助类
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 校验是否为有效的 JSON 字符串
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>是否有效</returns>
        public static bool IsValidJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return false;
            }

            json = json.Trim();
            if ((json.StartsWith("{") && json.EndsWith("}")) || // For object
                (json.StartsWith("[") && json.EndsWith("]"))) // For array
            {
                try
                {
                    var obj = JsonConvert.DeserializeObject(json);
                    return true;
                }
                catch (JsonException)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 将对象序列化为 JSON 字符串
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="value">对象实例</param>
        /// <returns>JSON 字符串</returns>
        public static string ObjToJson<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }

        /// <summary>
        /// 将 JSON 字符串反序列化为对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="json">JSON 字符串</param>
        /// <returns>对象实例</returns>
        public static T? JsonToObj<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (JsonException)
            {
                return default;
            }
        }
    }
}
