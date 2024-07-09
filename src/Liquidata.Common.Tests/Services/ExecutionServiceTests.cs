using Liquidata.Common.Services;
using Liquidata.Common.Services.Interfaces;
using Moq;
using System.Diagnostics;

namespace Liquidata.Common.Tests.Services
{
    public class ExecutionServiceTests
    {
        [Fact]
        public void GivenExecutionService_WhenCloned_ThenPropertiesSet()
        {
            Func<IBrowserService, Task> addAction = b => Task.CompletedTask;
            Func<IBrowserService, Task> removeAction = b => Task.CompletedTask;
            var selection = "XYZ";

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = addAction,
                BrowserPageRemovedAction = removeAction,
                CurrentSelection = selection,                
            };

            var clone = executionService.Clone() as ExecutionService;

            Assert.Equal(executionService.Browser, clone!.Browser);
            Assert.Equal(executionService.Concurrency, clone.Concurrency);
            Assert.Equal(executionService.DataHandler, clone.DataHandler);
            Assert.Equal(executionService.Project, clone.Project);
            Assert.Equal(executionService.XPathProcessor, clone.XPathProcessor);
            Assert.Equal(executionService.BrowserPageAddedAction, clone.BrowserPageAddedAction);
            Assert.Equal(executionService.BrowserPageRemovedAction, clone.BrowserPageRemovedAction);
            Assert.Equal(executionService.CurrentSelection, clone.CurrentSelection);
        }

        [Fact]
        public void GivenExecutionService_WhenClonedWithSelection_ThenPropertiesSet()
        {
            Func<IBrowserService, Task> addAction = b => Task.CompletedTask;
            Func<IBrowserService, Task> removeAction = b => Task.CompletedTask;
            var selection = "XYZ";

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = addAction,
                BrowserPageRemovedAction = removeAction,
                CurrentSelection = selection,
            };

            var newSelection = "NewSelection";
            var clone = executionService.Clone(newSelection) as ExecutionService;

            Assert.Equal(executionService.Browser, clone!.Browser);
            Assert.Equal(executionService.Concurrency, clone.Concurrency);
            Assert.Equal(executionService.DataHandler, clone.DataHandler);
            Assert.Equal(executionService.Project, clone.Project);
            Assert.Equal(executionService.XPathProcessor, clone.XPathProcessor);
            Assert.Equal(executionService.BrowserPageAddedAction, clone.BrowserPageAddedAction);
            Assert.Equal(executionService.BrowserPageRemovedAction, clone.BrowserPageRemovedAction);
            Assert.Equal(newSelection, clone.CurrentSelection);
        }

        [Fact]
        public void GivenExecutionService_WhenClonedWithBrowser_ThenPropertiesSet()
        {
            Func<IBrowserService, Task> addAction = b => Task.CompletedTask;
            Func<IBrowserService, Task> removeAction = b => Task.CompletedTask;
            var selection = "XYZ";

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = addAction,
                BrowserPageRemovedAction = removeAction,
                CurrentSelection = selection,
            };

            var newBrowser = new Mock<IBrowserService>().Object;
            var clone = executionService.Clone(browser: newBrowser) as ExecutionService;

            Assert.Equal(newBrowser, clone!.Browser);
            Assert.Equal(executionService.Concurrency, clone.Concurrency);
            Assert.Equal(executionService.DataHandler, clone.DataHandler);
            Assert.Equal(executionService.Project, clone.Project);
            Assert.Equal(executionService.XPathProcessor, clone.XPathProcessor);
            Assert.Equal(executionService.BrowserPageAddedAction, clone.BrowserPageAddedAction);
            Assert.Equal(executionService.BrowserPageRemovedAction, clone.BrowserPageRemovedAction);
            Assert.Equal(executionService.CurrentSelection, clone.CurrentSelection);
        }

        [Fact]
        public void GivenExecutionService_WhenClonedWithDataHandler_ThenPropertiesSet()
        {
            Func<IBrowserService, Task> addAction = b => Task.CompletedTask;
            Func<IBrowserService, Task> removeAction = b => Task.CompletedTask;
            var selection = "XYZ";

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = addAction,
                BrowserPageRemovedAction = removeAction,
                CurrentSelection = selection,
            };

            var newDataHandler = new Mock<IDataHandlerService>().Object;
            var clone = executionService.Clone(dataHandler: newDataHandler) as ExecutionService;

            Assert.Equal(executionService.Browser, clone!.Browser);
            Assert.Equal(executionService.Concurrency, clone.Concurrency);
            Assert.Equal(newDataHandler, clone.DataHandler);
            Assert.Equal(executionService.Project, clone.Project);
            Assert.Equal(executionService.XPathProcessor, clone.XPathProcessor);
            Assert.Equal(executionService.BrowserPageAddedAction, clone.BrowserPageAddedAction);
            Assert.Equal(executionService.BrowserPageRemovedAction, clone.BrowserPageRemovedAction);
            Assert.Equal(executionService.CurrentSelection, clone.CurrentSelection);
        }

        [Fact]
        public async Task GivenExecutionService_WhenNoExecutionTasks_ThenWaitForExecutionHasNoWait()
        {
            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
            };

            executionService.Initialize();

            var timer = Stopwatch.StartNew();
            await executionService.WaitForExecutionTasksAsync();

            Assert.True(timer.ElapsedMilliseconds <= 250);
        }

        [Fact]
        public async Task GivenExecutionService_WhenExecutionTasks_ThenWaitForExecutionHasWait()
        {
            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
            };

            executionService.Initialize();
            await executionService.CreateExecutionTaskAsync(() => Task.Delay(1000));

            var timer = Stopwatch.StartNew();
            await executionService.WaitForExecutionTasksAsync();

            Assert.True(timer.ElapsedMilliseconds >= 1000);
        }

        [Fact]
        public async Task GivenExecutionService_WhenLogError_ThenErrorLogged()
        {
            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
            };

            var message = Guid.NewGuid().ToString();
            await executionService.LogErrorAsync(message);

            Assert.Single(executionService.LoggedErrors);
            Assert.Contains(executionService.LoggedErrors, x => x == message);
        }

        [Fact]
        public async Task GivenExecutionService_WhenLogMessage_ThenMessageLogged()
        {
            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
            };

            var message = Guid.NewGuid().ToString();
            await executionService.LogMessageAsync(message);

            Assert.Single(executionService.LoggedMessages);
            Assert.Contains(executionService.LoggedMessages, x => x == message);
        }

        [Fact]
        public async Task GivenExecutionService_WhenBrowserAdded_ThenAddActionTriggered()
        {
            var isAdded = false;
            var isRemoved = false;

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = b => { isAdded = true; return Task.CompletedTask; },
                BrowserPageRemovedAction = b => { isRemoved = true; return Task.CompletedTask; },
            };

            await executionService.RegisterBrowserAsync(new Mock<IBrowserService>().Object);

            Assert.True(isAdded);
            Assert.False(isRemoved);
        }

        [Fact]
        public async Task GivenExecutionService_WhenBrowserRemove_ThenRemoveActionTriggered()
        {
            var isAdded = false;
            var isRemoved = false;

            var executionService = new ExecutionService
            {
                Browser = new Mock<IBrowserService>().Object,
                Concurrency = 5,
                DataHandler = new Mock<IDataHandlerService>().Object,
                Project = new Mock<Project>().Object,
                XPathProcessor = new Mock<IXPathProcessorService>().Object,
                BrowserPageAddedAction = b => { isAdded = true; return Task.CompletedTask; },
                BrowserPageRemovedAction = b => { isRemoved = true; return Task.CompletedTask; },
            };

            await executionService.UnregisterBrowserAsync(new Mock<IBrowserService>().Object);

            Assert.True(isRemoved);
            Assert.False(isAdded);
        }
    }
}
