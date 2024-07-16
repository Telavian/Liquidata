using Liquidata.Common.Services.Interfaces;
using System.Collections.Concurrent;

namespace Liquidata.Common.Services;

public class ExecutionService : IExecutionService
{
    private bool _isRunning;

    private ConcurrentQueue<Func<Task>> _executionTasks = new ConcurrentQueue<Func<Task>>();
    private Task[] _taskExecutors = [];            

    public required IDataHandlerService DataHandler { get; init; }
    public required IXPathProcessorService XPathProcessor { get; init; }
    public required IBrowserService Browser { get; init; }
    public required int Concurrency { get; init; }        
    public required Project Project { get; init; }
    public Func<IBrowserService, Task> BrowserPageAddedAction { get; init; } = b => Task.CompletedTask;
    public Func<IBrowserService, Task> BrowserPageRemovedAction { get; init; } = b => Task.CompletedTask;

    public string CurrentSelection { get; set; } = "";

    public IList<string> LoggedErrors { get; private set; } = new List<string>();
    public IList<string> LoggedMessages { get; private set; } = new List<string>();
    public IList<IBrowserService> AllBrowsers { get; private set; } = new List<IBrowserService>();

    public void Initialize()
    {
        _isRunning = true;
        _taskExecutors ??= BuildTaskExecutors(Concurrency);        
    }

    public IExecutionService Clone(string? selection = null, IBrowserService? browser = null, IDataHandlerService? dataHandler = null)
    {
        return new ExecutionService
        {
            _executionTasks = _executionTasks,
            _taskExecutors = _taskExecutors,

            Project = Project,
            Browser = browser ?? Browser,
            DataHandler = dataHandler ?? DataHandler,
            XPathProcessor = XPathProcessor,
            BrowserPageAddedAction = BrowserPageAddedAction,
            BrowserPageRemovedAction = BrowserPageRemovedAction,

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
            // Signal finished
            if (_executionTasks.IsEmpty)
            {
                _isRunning = false;
            }

            // Check if actually finished
            if (_taskExecutors.Length == 0 || _taskExecutors.All(x => x.IsCompleted))
            {
                return;
            }

            await Task.Delay(100);           
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

        await BrowserPageAddedAction(browser);
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

        await BrowserPageRemovedAction(browser);
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
