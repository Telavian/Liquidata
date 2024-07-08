using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Execution;

namespace Liquidata.Common.Services.Interfaces;

public interface IDataHandlerService
{
    public string DataScope { get; set; }

    public IDataHandlerService Clone();
    public Task MergeDataAsync(IDataHandlerService dataHandler);

    public Task AddRecordAsync();
    public Task AddDataAsync(string name, string value);
    
    Task AddScreenshotAsync(string name, byte[] screenshot);
    ExecutionResults GetExecutionResults();
    public Task<string> CleanDataAsync(string data, FieldType fieldType);
}
