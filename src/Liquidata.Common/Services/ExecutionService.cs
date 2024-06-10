using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Services
{
    public class ExecutionService(Project project, IBrowserService browser, IDataHandlerService dataHandler, IXPathProcessorService xPathProcessor) : IExecutionService
    {
        public IDataHandlerService DataHandler => dataHandler;
        public IBrowserService Browser => browser;
        public IXPathProcessorService XPathProcessor => xPathProcessor;

        public string CurrentSelection { get; set; } = "";
        public Project CurrentProject => project;    

        public IExecutionService Clone(string? selection = null, IBrowserService? browser = null)
        {
            return new ExecutionService(project, browser ?? Browser, dataHandler, xPathProcessor)
            {
                CurrentSelection = selection ?? CurrentSelection
            };
        }

        public Task CreateExecutionTaskAsync(Func<Task> action)
        {
            throw new NotImplementedException();
        }

        public Task WaitForExecutionTasksAsync()
        {
            throw new NotImplementedException();
        }

        public Task LogErrorAsync(string message)
        {
            throw new NotImplementedException();
        }

        public Task LogMessageAsync(string message)
        {
            throw new NotImplementedException();
        }
    }
}
