namespace Liquidata.Common.Services.Interfaces
{
    public interface IExecutionService
    {
        public IDataHandlerService DataHandler { get; }
        public IBrowserService Browser { get; }
        public IXPathProcessorService XPathProcessor { get; }

        public string CurrentSelection { get; }
        public Project CurrentProject { get; }

        public Task CreateExecutionTaskAsync(Func<Task> action);
        public Task LogErrorAsync(string message);
        public Task LogMessageAsync(string message);
        public IExecutionService Clone(string? selection = null, IBrowserService? browser = null);
    }
}
