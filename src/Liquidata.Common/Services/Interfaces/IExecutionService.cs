namespace Liquidata.Common.Services.Interfaces
{
    public interface IExecutionService
    {
        public IDataHandlerService DataHandler { get; }
        public IBrowserService Browser { get; }
        public IXPathProcessorService XPathProcessor { get; }

        public int Concurrency { get; }
        public string CurrentSelection { get; set; }
        public Project Project { get; }
        IList<string> LoggedErrors { get; }
        IList<string> LoggedMessages { get; }
        IList<IBrowserService> AllBrowsers { get; }

        public Task RegisterBrowserAsync(IBrowserService browser);
        public Task UnregisterBrowserAsync(IBrowserService browser);
        public Task CreateExecutionTaskAsync(Func<Task> action);
        public Task WaitForExecutionTasksAsync();
        public Task LogErrorAsync(string message);
        public Task LogMessageAsync(string message);
        public IExecutionService Clone(string? selection = null, IBrowserService? browser = null);
    }
}
