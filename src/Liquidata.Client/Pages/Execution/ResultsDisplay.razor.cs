using BlazorComponentBus;
using Liquidata.Client.Messages;
using Liquidata.Common.Execution;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Execution;

public class ResultsDisplayViewModel : ViewModelBase, IDisposable
{
    [Inject] private ComponentBus _bus { get; set; } = null!;

    [Parameter] public ExecutionResults ExecutionResults { get; set; } = null!;

    public void Dispose()
    {
        _bus.UnSubscribe<ExecutionResultsUpdatedMessage>(HandleExecutionResultsUpdatedMessage);
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Yield();
        _bus.Subscribe<ExecutionResultsUpdatedMessage>(HandleExecutionResultsUpdatedMessage);
        await base.OnInitializedAsync();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "<Pending>")]
    private async void HandleExecutionResultsUpdatedMessage(MessageArgs message)
    {
        var typedMessage = message.GetMessage<ExecutionResultsUpdatedMessage>();
        if (typedMessage is null)
        {
            return;
        }

        await RefreshAsync();
    }
}
