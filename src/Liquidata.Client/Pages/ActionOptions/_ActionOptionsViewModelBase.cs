using BlazorComponentBus;
using Liquidata.Client.Messages;
using Liquidata.Common.Actions.Shared;
using Liquidata.UI.Common.Pages.Common;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public abstract class ActionOptionsViewModelBase<T> : ViewModelBase, IDisposable
    where T : ActionBase
{
    [Inject] private ComponentBus _bus { get; set; } = null!;

    [Parameter] public ActionBase? Action { get; set; }
    [Parameter] public EditProjectViewModel? Parent { get; set; }

    public T TypedAction => (T)Action!;

    public string Name
    {
        get => TypedAction.Name;
        set
        {
            TypedAction.Name = value;
            _ = ActionUpdatedAsync();
        }
    }

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

    protected async Task ActionUpdatedAsync()
    {
        await Task.Yield();
        await _bus.Publish(new ActionUpdatedMessage { ActionId = Action?.ActionId ?? Guid.Empty });
        await RefreshAsync();
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
