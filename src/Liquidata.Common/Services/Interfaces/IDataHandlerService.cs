using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Services.Interfaces
{
    public interface IDataHandlerService
    {
        public void AddRecord();
        public void AddData(string name, string value);

        public Task<string> CleanDataAsync(string data, FieldType fieldType);
        Task AddDataScopeAsync(string name);
        Task RemoveDataScopeAsync();
        Task AddScreenshotAsync(string name, byte[] screenshot);
    }
}
