using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace Apteryx.Routing.Role.Authority
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Serialize<T>(obj, jsonOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="jsonOptions"></param>
        /// <returns></returns>
        public static string ToJson<T>(this T obj, JsonSerializerOptions jsonOptions)
        {
            return JsonSerializer.Serialize<T>(obj, jsonOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <returns></returns>
        public static T? FromJson<T>(this string s)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Deserialize<T>(s, jsonOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="jsonOptions"></param>
        /// <returns></returns>
        public static T? FromJson<T>(this string s, JsonSerializerOptions jsonOptions)
        {
            return JsonSerializer.Deserialize<T>(s, jsonOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? FromJson(this string s, Type type)
        {
            JsonSerializerOptions jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
            };
            return JsonSerializer.Deserialize(s, type, jsonOptions);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="s"></param>
        /// <param name="type"></param>
        /// <param name="jsonOptions"></param>
        /// <returns></returns>
        public static object? FromJson(this string s, Type type, JsonSerializerOptions jsonOptions)
        {
            return JsonSerializer.Deserialize(s, type, jsonOptions);
        }

    }
}
