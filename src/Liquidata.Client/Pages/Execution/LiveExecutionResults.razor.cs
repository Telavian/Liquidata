﻿using BlazorComponentBus;
using BlazorFileSaver;
using DebounceThrottle;
using Liquidata.Client.Messages;
using Liquidata.Common;
using Liquidata.Common.Execution;
using Liquidata.Common.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Text.Json.Serialization;
using System.Text.Json;
using Liquidata.UI.Common.Pages.Common;
using Liquidata.Common.Services;
using Liquidata.Common.Extensions;

namespace Liquidata.Client.Pages.Execution;

public class LiveExecutionResultsViewModel : ViewModelBase, IDisposable
{
    private DebounceDispatcher _projectExecutionDebounce = new DebounceDispatcher(1000);
    [Inject] private ComponentBus _bus { get; set; } = null!;
    [Inject] private IJSRuntime? _jsRuntime { get; set; } = null!;
    [Inject] private IBlazorFileSaver _blazorFileSaver { get; set; } = null!;

    [Parameter] public bool ShowResults { get; set; } = true;
    [Parameter] public bool ShowLogs { get; set; } = false;        

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
            await ExecuteProjectAsync(typedMessage.Project, typedMessage.AllowInteractive, typedMessage.ExecutionService);
            await RefreshAsync();
        });
    }

    private async Task ExecuteProjectAsync(Project project, bool allowInteractive, IExecutionService executionService)
    {
        var shouldContinue = await CheckValidExecutionStateAsync(project, allowInteractive);

        if (!shouldContinue) 
        {
            return;
        }

        ExecutionMessage = "";
        IsResultsLoading = true;
        await RefreshAsync();

        await PerformProjectExecutionAsync(project, executionService);
    }

    private async Task PerformProjectExecutionAsync(Project project, IExecutionService executionService)
    {
        try
        {
            Console.WriteLine($"Executing project '{project.Name}'");
            await project.ExecuteProjectAsync(executionService);

            await executionService.WaitForExecutionTasksAsync();

            var executionResults = executionService.DataHandler.GetExecutionResults();
            executionResults.LoggedMessages = executionService.LoggedMessages
                .ToArray();

            if (executionResults.Records.Length == 0)
            {
                ExecutionMessage = "No records extracted";
            }
            else
            {
                ExecutionResults = executionResults;
            }
        }
        catch (Exception ex)
        {
            ExecutionMessage = $"Error while executing project: {ex.Message}";
        }
        finally
        {
            IsResultsLoading = false;
            await RefreshAsync();

            await _bus.Publish(new ExecutionResultsUpdatedMessage());
        }
    }

    private async Task<bool> CheckValidExecutionStateAsync(Project project, bool allowInteractive)
    {
        if (project == null)
        {
            ExecutionMessage = "Project is not loaded";
            ExecutionResults = null;

            await RefreshAsync();
            return false;
        }

        var isInteractive = project.CheckIfInteractive();

        if (!allowInteractive && isInteractive)
        {
            ExecutionMessage = "Project is interactive. To see parser results execute project.";
            ExecutionResults = null;

            await RefreshAsync();
            return false;
        }

        var isFullyDefined = project.CheckIfFullyDefined();

        if (!isFullyDefined)
        {
            ExecutionMessage = "Project contains errors.";
            ExecutionResults = null;

            await RefreshAsync();
            return false;
        }

        return true;
    }

    private async Task HandleSaveExecutionResultsAsync()
    {
        if (ExecutionResults is null)
        {
            return;
        }

        var json = ExecutionResults.ToJson();
        await _blazorFileSaver.SaveAs($"extraction_results.json", json, "application/json");
    }
}
