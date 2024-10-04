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
        /// <param name="jsonString">JSON 字符串</param>
        /// <returns>是否有效</returns>
        public static bool IsValidJson(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString))
            {
                return false;
            }

            try
            {
                var obj = JsonToObj<object>(jsonString);
                return obj != null;
            }
            catch (JsonException)
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
        /// <param name="jsonString">JSON 字符串</param>
        /// <returns>对象实例</returns>
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
