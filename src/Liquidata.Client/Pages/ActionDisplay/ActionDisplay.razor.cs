using Liquidata.Client.Extensions;
using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class ActionDisplayViewModel : ViewModelBase
{
    [Parameter]
    public ActionBase? Action { get; set; }

    [Parameter]
    public EditProjectViewModel? Parent { get; set; }

    public bool IsMouseOver { get; set; }

    private Func<bool, Task>? _updateIsMouseOverAsyncCommand;
    public Func<bool, Task> UpdateIsMouseOverAsyncCommand => _updateIsMouseOverAsyncCommand ??= CreateEventCallbackAsyncCommand<bool>(UpdateIsMouseOverAsync, "Unable to update is mouse over");


    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        // TODO: Optimize
        await RefreshAsync();
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

    private async Task UpdateIsMouseOverAsync(bool isOver)
    {
        await Task.Yield();
        IsMouseOver = isOver;
    }
}
