using BlazorComponentBus;
using DebounceThrottle;
using Liquidata.Client.Messages;
using Liquidata.Client.Pages.Common;
using Liquidata.Client.Services;
using Liquidata.Common;
using Liquidata.Common.Execution;
using Liquidata.Common.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Liquidata.Client.Pages.Execution
{
    public class LiveExecutionResultsViewModel : ViewModelBase, IDisposable
    {
        private DebounceDispatcher _projectExecutionDebounce = new DebounceDispatcher(500);
        [Inject] private ComponentBus _bus { get; set; } = null!;
        [Inject] private IJSRuntime? _jsRuntime { get; set; } = null!;

        public string ExecutionMessage { get; set; } = "";
        public ExecutionResults? ExecutionResults { get; set; } = null!;
        public bool IsResultsLoading { get; set; } = false;

        public void Dispose()
        {
            _bus.UnSubscribe<ExecuteProjectMessage>(HandleExecuteProjectMessage);
        }

        protected override async Task OnInitializedAsync()
        {
            await Task.Yield();
            _bus.Subscribe<ExecuteProjectMessage>(HandleExecuteProjectMessage);
            await base.OnInitializedAsync();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "<Pending>")]
        private async void HandleExecuteProjectMessage(MessageArgs message)
        {
            var typedMessage = message.GetMessage<ExecuteProjectMessage>();
            if (typedMessage is null || typedMessage.Project is null)
            {
                return;
            }

            await Task.Yield();
            await _projectExecutionDebounce.DebounceAsync(async () =>
            {
                await ExecuteProjectAsync(typedMessage.Project);
                await RefreshAsync();
            });
        }

        private async Task ExecuteProjectAsync(Project project)
        {
            if (project == null)
            {
                ExecutionMessage = "Project is not loaded";
                await RefreshAsync();
                return;
            }

            var isInteractive = project.CheckIfInterative();

            if (isInteractive)
            {
                ExecutionMessage = "Project is interactive. To see parser results execute project.";
                await RefreshAsync();
                return;
            }

            var isFullyDefined = project.CheckIfFullyDefined();

            if (!isFullyDefined)
            {
                ExecutionMessage = "Project contains errors.";
                await RefreshAsync();
                return;
            }

            ExecutionMessage = "";
            IsResultsLoading = true;
            await RefreshAsync();
            
            try
            {
                var browser = new ClientBrowserService(_jsRuntime!);
                var dataHandler = new DataHandlerService();
                var xPathProcessor = new XPathProcessorService(browser);

                var executionService = new ExecutionService(project, 1, browser, dataHandler, xPathProcessor);                
                await project.ExecuteProjectAsync(executionService);

                await executionService.WaitForExecutionTasksAsync();
                ExecutionResults = dataHandler.GetExecutionResults();
            }
            catch (Exception ex)
            {
                ExecutionMessage = $"Error while executing project: {ex.Message}";
            }
            finally
            {
                IsResultsLoading = false;
                await RefreshAsync();
            }
        }
    }
}
