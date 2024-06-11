using BlazorComponentBus;
using BlazorFileSaver;
using DebounceThrottle;
using Liquidata.Client.Messages;
using Liquidata.Client.Pages.Common;
using Liquidata.Client.Pages.Dialogs;
using Liquidata.Client.Services;
using Liquidata.Common;
using Liquidata.Common.Execution;
using Liquidata.Common.Services;
using Liquidata.Common.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace Liquidata.Client.Pages.Execution
{
    public class LiveExecutionResultsViewModel : ViewModelBase, IDisposable
    {
        private DebounceDispatcher _projectExecutionDebounce = new DebounceDispatcher(500);
        [Inject] private ComponentBus _bus { get; set; } = null!;
        [Inject] private IJSRuntime? _jsRuntime { get; set; } = null!;
        [Inject] private IBlazorFileSaver _blazorFileSaver { get; set; } = null!;

        public string ExecutionMessage { get; set; } = "";
        public ExecutionResults? ExecutionResults { get; set; } = null!;
        public bool IsResultsLoading { get; set; } = false;

        private Func<Task>? _saveExecutionResultsAsyncCommand;
        public Func<Task> SaveExecutionResultsAsyncCommand => _saveExecutionResultsAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleSaveExecutionResultsAsync, "Unable to save execution results");

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
                ExecutionResults = null;

                await RefreshAsync();
                return;
            }

            var isInteractive = project.CheckIfInterative();

            if (isInteractive)
            {
                ExecutionMessage = "Project is interactive. To see parser results execute project.";
                ExecutionResults = null;

                await RefreshAsync();
                return;
            }

            var isFullyDefined = project.CheckIfFullyDefined();

            if (!isFullyDefined)
            {
                ExecutionMessage = "Project contains errors.";
                ExecutionResults = null;

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

        private async Task HandleSaveExecutionResultsAsync()
        {
            if (ExecutionResults is null)
            {
                return;
            }

            var options = new JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());

            var json = JsonSerializer.Serialize(ExecutionResults, options);
            await _blazorFileSaver.SaveAs($"extraction_results.json", json, "application/json");
        }
    }
}
