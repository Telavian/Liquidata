using BlazorComponentBus;
using Liquidata.Client.Extensions;
using Liquidata.Client.Messages;
using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class ActionDisplayViewModel : ViewModelBase, IDisposable
{
    [Inject] private ComponentBus _bus { get; set; } = null!;

    [Parameter]
    public ActionBase? Action { get; set; }

    [Parameter]
    public EditProjectViewModel? Parent { get; set; }

    public bool IsMouseOver { get; set; }

    public bool IsDisabled
    {
        get => Action!.IsDisabled;
        set
        {
            Action!.IsDisabled = value;
            _ = ActionUpdatedAsync();
        }
    }

    private Func<bool, Task>? _updateIsMouseOverAsyncCommand;
    public Func<bool, Task> UpdateIsMouseOverAsyncCommand => _updateIsMouseOverAsyncCommand ??= CreateEventCallbackAsyncCommand<bool>(UpdateIsMouseOverAsync, "Unable to update is mouse over");

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

    protected string BuildItemVisibility(params bool[] states)
    {
        var anyTrue = states.Any(x => x);

        var style = @"margin-right: -15px; ";

        return anyTrue
            ? $"{style} display: block"
            : $"{style} display: none";
    }

    protected string BuildActionIcon()
    {
        return Action!.ActionType.BuildActionIcon();
    }

    protected Color BuildActionColor()
    {
        return Action!.ActionType.BuildActionColor();
    }

    protected string BuildValidationVisibility()
    {
        var errors = Action!.BuildValidationErrors();
        var isInvalid = !Action.IsDisabled && errors.Length > 0;
        return BuildItemVisibility(isInvalid);
    }

    protected MarkupString BuildValidationErrors()
    {
        var errors = Action!.BuildValidationErrors();        

        if (errors.Length == 0)
        {
            return (MarkupString)"";
        }

        var formattedErrors = errors
            .Select(x => $"<br>{x}");
        return (MarkupString)$"Validation errors: {string.Join("", formattedErrors)}";
    }


    protected async Task ActionUpdatedAsync()
    {
        await Task.Yield();
        await _bus.Publish(new ActionUpdatedMessage { ActionId = Action?.ActionId ?? Guid.Empty });
        await RefreshAsync();
    }

    private async Task UpdateIsMouseOverAsync(bool isOver)
    {
        await Task.Yield();
        IsMouseOver = isOver;
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
