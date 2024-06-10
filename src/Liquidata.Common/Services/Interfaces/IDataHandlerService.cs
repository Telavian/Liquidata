using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Execution;

namespace Liquidata.Common.Services.Interfaces
{
    public interface IDataHandlerService
    {
        public string DataScope { get; set; }

        public void AddRecord();
        public void AddData(string name, string value);
        
        Task AddScreenshotAsync(string name, byte[] screenshot);
        ExecutionResults GetExecutionResults();
        public Task<string> CleanDataAsync(string data, FieldType fieldType);
    }
}
