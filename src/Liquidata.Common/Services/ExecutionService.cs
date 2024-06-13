using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Services.Interfaces;
using System.Collections.Concurrent;

namespace Liquidata.Common.Services
{
    public class ExecutionService : IExecutionService
    {
        private ConcurrentQueue<Func<Task>> _executionTasks = new ConcurrentQueue<Func<Task>>();
        private Task[] _taskExecutors = null!;
        private Project _project = null!;
        private IBrowserService _browser = null!;
        private IDataHandlerService _dataHandler = null!;
        private IXPathProcessorService _xPathProcessor = null!;
        private Func<Task> _browserRegistrationAction = () => Task.CompletedTask;

        private bool _isRunning = true;

        public IDataHandlerService DataHandler => _dataHandler;
        public IBrowserService Browser => _browser;
        public IXPathProcessorService XPathProcessor => _xPathProcessor;

        public int Concurrency { get; private set; }
        public string CurrentSelection { get; set; } = "";
        public Project CurrentProject => _project;

        public IList<string> LoggedErrors { get; private set; } = new List<string>();
        public IList<string> LoggedMessages { get; private set; } = new List<string>();
        public IList<IBrowserService> AllBrowsers { get; private set; } = new List<IBrowserService>();

        public ExecutionService(Project project, int concurrency, IBrowserService browser, IDataHandlerService dataHandler, IXPathProcessorService xPathProcessor, Func<Task> browserRegistrationAction)
        {
            _project = project;
            _browser = browser;
            _dataHandler = dataHandler;
            _xPathProcessor = xPathProcessor;
            _browserRegistrationAction = browserRegistrationAction;

            _taskExecutors = BuildTaskExecutors(concurrency);
            Concurrency = concurrency;
        }

        private ExecutionService()
        {
            // Nothing
        }

        public IExecutionService Clone(string? selection = null, IBrowserService? browser = null)
        {
            return new ExecutionService()
            {
                _executionTasks = _executionTasks,
                _taskExecutors = _taskExecutors,
                _project = _project,
                _browser = browser ?? Browser,
                _dataHandler = _dataHandler,
                _xPathProcessor = _xPathProcessor,
                _browserRegistrationAction = _browserRegistrationAction,
                LoggedErrors = LoggedErrors,
                LoggedMessages = LoggedMessages,
                AllBrowsers = AllBrowsers,
                Concurrency = Concurrency,
                CurrentSelection = selection ?? CurrentSelection
            };
        }

        public async Task CreateExecutionTaskAsync(Func<Task> action)
        {
            await Task.Yield();
            _executionTasks.Enqueue(action);
        }

        public async Task WaitForExecutionTasksAsync()
        {
            while (true)
            {
                await Task.Delay(100);
                
                if (_executionTasks.IsEmpty)
                {
                    _isRunning = false;
                    return;
                }
            }
        }

        public async Task LogErrorAsync(string message)
        {
            await Task.Yield();
            lock (LoggedErrors)
            {
                LoggedErrors.Add(message);
            }
        }

        public async Task LogMessageAsync(string message)
        {
            await Task.Yield();

            lock (LoggedMessages)
            {
                LoggedMessages.Add(message);
            }
        }

        public async Task RegisterBrowserAsync(IBrowserService browser)
        {
            await Task.Yield();                        
            lock (AllBrowsers)
            {
                if (!AllBrowsers.Contains(browser))
                {                    
                    AllBrowsers.Add(browser);                    
                }
            }

            await _browserRegistrationAction();
        }

        public async Task UnregisterBrowserAsync(IBrowserService browser)
        {
            await Task.Yield();
            
            lock (AllBrowsers)
            {
                // Always keep the first
                if (AllBrowsers.Count > 1)
                {
                    AllBrowsers.Remove(browser);
                }
            }

            await _browserRegistrationAction();
        }

        private Task[] BuildTaskExecutors(int concurrency)
        {
            return Enumerable.Range(0, concurrency)
                .Select(x => HandleExecutionTaskAsync())
                .ToArray();
        }

        private async Task HandleExecutionTaskAsync()
        {
            while (_isRunning)
            {
                await Task.Delay(100);
                var isFound = _executionTasks.TryDequeue(out var task);

                if (isFound) 
                {
                    await task!();
                }
            }
        }
    }
}
