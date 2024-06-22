using System.Text.Json;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson<T>(this T item, bool isFormatted = false)
        {
            var options = BuildSerializerOptions(isFormatted);
            return JsonSerializer.Serialize(item, options);
        }

        public static T? FromJson<T>(this string json)
        {
            var options = BuildSerializerOptions();
            return JsonSerializer.Deserialize<T>(json, options);
        }

        public static ValueTask<T?> FromJsonAsync<T>(this Stream jsonStream)
        {
            var options = BuildSerializerOptions();
            return JsonSerializer.DeserializeAsync<T>(jsonStream, options);
        }


        private static JsonSerializerOptions BuildSerializerOptions(bool isFormatted = false)
        {
            var options = new JsonSerializerOptions();
            options.WriteIndented = isFormatted;

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
