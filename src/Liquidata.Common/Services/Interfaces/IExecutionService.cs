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
    }
}
