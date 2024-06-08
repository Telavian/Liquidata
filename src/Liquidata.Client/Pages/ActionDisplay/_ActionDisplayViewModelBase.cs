using BlazorComponentBus;
using Liquidata.Client.Extensions;
using Liquidata.Client.Messages;
using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.ActionDisplay;

public abstract class ActionDisplayViewModelBase<T> : ViewModelBase, IDisposable
    where T : ActionBase
{
    [Inject]
    private ComponentBus _bus { get; set; } = null!;

    [Parameter]
    public ActionBase? Action { get; set; }

    [Parameter]
    public EditProjectViewModel? Parent { get; set; }

    public T TypedAction => (T)Action!;

    public void Dispose()
    {
        _bus.UnSubscribe<ActionUpdatedMessage>(HandleActionUpdatedMessage);
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // TODO: Optimize
        await RefreshAsync();

        _bus.Subscribe<ActionUpdatedMessage>(HandleActionUpdatedMessage);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("AsyncUsage", "AsyncFixer03:Fire-and-forget async-void methods or delegates", Justification = "<Pending>")]
    private async void HandleActionUpdatedMessage(MessageArgs message)
    {
        var typedMessage = message.GetMessage<ActionUpdatedMessage>();
        if (typedMessage is null || typedMessage.ActionId != Action?.ActionId)
        {
            return;
        }

        await Task.Yield();
        await RefreshAsync();
    }
}
