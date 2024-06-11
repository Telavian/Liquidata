using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Execution;
using Liquidata.Common.Services.Interfaces;
using Microsoft.Recognizers.Text;
using Microsoft.Recognizers.Text.Choice;
using Microsoft.Recognizers.Text.DateTime;
using Microsoft.Recognizers.Text.Number;
using Microsoft.Recognizers.Text.Sequence;
using System.Xml.Schema;

namespace Liquidata.Common.Services
{
    public class DataHandlerService : IDataHandlerService
    {
        private DataRecord? _currentRecord = null;
        private List<DataRecord> _allRecords = [];

        private readonly Dictionary<string, byte[]> _screenshots = new Dictionary<string, byte[]>();

        public string DataScope { get; set; } = "";

        public void AddData(string name, string value)
        {
            if (_currentRecord == null)
            {
                AddRecord();
            }

            if (!string.IsNullOrWhiteSpace(DataScope))
            {
                name = $"{DataScope}.{name}";
            }

            _currentRecord!.AddData(name, value);
        }

        public void AddRecord()
        {
            if (_currentRecord != null && !_currentRecord.HasData())
            {
                return;
            }

            _currentRecord = new DataRecord();
            _allRecords.Add(_currentRecord);
        }

        public async Task AddScreenshotAsync(string name, byte[] screenshot)
        {
            await Task.Yield();
            name = $"name_{DateTime.Now.ToString("s")}";
            _screenshots.Add(name, screenshot);
        }

        public ExecutionResults GetExecutionResults()
        {
            var columns = _allRecords
                .SelectMany(x => x.AllColumns)
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            return new ExecutionResults
            {
                AllColumns = columns,
                Records = _allRecords.ToArray(),
                Screenshots = _screenshots
                    .Select(x => new Screenshot { Name = x.Key, Data = x.Value })
                    .ToArray()
            };
        }

        public async Task<string> CleanDataAsync(string data, FieldType fieldType)
        {
            await Task.Yield();

            if (fieldType == FieldType.Text)
            {
                // TODO: Do anything with it?
                return data;
            }

            if (fieldType == FieldType.Boolean)
            {
                return ParseBoolean(data);
            }

            if (fieldType == FieldType.Datetime)
            {
                return ParseDateTime(data);
            }

            if (fieldType == FieldType.Numeric)
            {
                return ParseNumeric(data);
            }

            if (fieldType == FieldType.Url)
            {
                return ParseUrl(data);
            }

            throw new Exception($"Unknown field type: {fieldType}");
        }

        private string ParseBoolean(string data)
        {
            var isValid = bool.TryParse(data, out var value);
            if (isValid)
            {
                return value.ToString();
            }

            var result = ChoiceRecognizer.RecognizeBoolean(data, Culture.English);
            return GetResolvedValue(result);
        }

        private string ParseDateTime(string data)
        {
            var isValid = DateTime.TryParse(data, out var value);
            if (isValid)
            {
                return value.ToString();
            }

            var result = DateTimeRecognizer.RecognizeDateTime(data, Culture.English);
            return GetResolvedValue(result);
        }

        private string ParseNumeric(string data)
        {
            var isValid = double.TryParse(data, out var value);
            if (isValid)
            {
                return value.ToString();
            }

            var result = NumberRecognizer.RecognizeNumber(data, Culture.English);
            return GetResolvedValue(result);
        }

        private string ParseUrl(string data)
        {
            var isValid = Uri.TryCreate(data, UriKind.RelativeOrAbsolute, out var value);
            if (isValid)
            {
                return value!.ToString();
            }

            var result = SequenceRecognizer.RecognizeURL(data, Culture.English);
            return GetResolvedValue(result);
        }

        private string GetResolvedValue(List<ModelResult> result)
        {
            var value = result
                ?.Where(x => x.Resolution.ContainsKey("value"))
                ?.Select(x => x.Resolution["value"].ToString())
                ?.FirstOrDefault() ?? "";

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            var values = result
                ?.Where(x => x.Resolution.ContainsKey("values"))
                ?.SelectMany(x => x.Resolution["values"] as List<Dictionary<string, string>>)
                ?.Where(x => x is not null && x.ContainsKey("value"))
                ?.Select(x => x["value"].ToString())
                ?.FirstOrDefault() ?? "";

            if (!string.IsNullOrWhiteSpace(values))
            {
                return values.ToString()!;
            }

            return "";
        }
    }
}
