using System.Text.Json;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Execution
{
    [JsonConverter(typeof(DataRecordJsonConverter))]
    public class DataRecord
    {
        private Dictionary<string, string> _data = new Dictionary<string, string>();

        [JsonIgnore] public string[] AllColumns => _data.Keys.ToArray();

        public bool HasData()
        {
            return _data.Count > 0;
        }

        public void AddData(string key, string value)
        {
            _data[key] = value;
        }

        public string? GetRowData(string column)
        {
            var isFound = _data.TryGetValue(column, out var data);

            return isFound
                ? data 
                : null;
        }

        private class DataRecordJsonConverter : JsonConverter<DataRecord>
        {
            public override DataRecord? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new DataRecord
                {
                    _data = JsonSerializer.Deserialize<Dictionary<string, string>>(ref reader)!
                };
            }

            public override void Write(Utf8JsonWriter writer, DataRecord value, JsonSerializerOptions options)
            {
                JsonSerializer.Serialize(writer, value._data, options);
            }
        }
    }
}
