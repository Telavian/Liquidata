using Liquidata.Client.Extensions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.Dialogs;

public class AddActionDialogViewModel : DialogViewModelBase
{
    public string _filterText = "";
    public string FilterText
    {
        get => _filterText;
        set => UpdateProperty(ref _filterText, value,
            v => FilterActionsAsync(v));
    }

    public ActionType[] FilteredActionTypes { get; set; } = Enum.GetValues<ActionType>();
    public ActionType? SelectedActionType { get; set; } = null!;

    private Func<ActionType, Task>? _selectActionTypeAsyncCommand;
    public Func<ActionType, Task> SelectActionTypeAsyncCommand => _selectActionTypeAsyncCommand ??= CreateEventCallbackAsyncCommand<ActionType>(HandleSelectActionTypeAsync, "Unable to select action type");

    protected string SelectionSectionVisibility(ActionType[] actionTypes)
    {
        var isVisible = actionTypes.Any(x => x.IsSelectionAction());
        return VisibilityToDisplay(isVisible);
    }

    protected string DataSectionVisibility(ActionType[] actionTypes)
    {
        var isVisible = actionTypes.Any(x => x.IsDataAction());
        return VisibilityToDisplay(isVisible);
    }

    protected string LogicSectionVisibility(ActionType[] actionTypes)
    {
        var isVisible = actionTypes.Any(x => x.IsLogicAction());
        return VisibilityToDisplay(isVisible);
    }

    protected string InteractionSectionVisibility(ActionType[] actionTypes)
    {
        var isVisible = actionTypes.Any(x => x.IsInteractionAction());
        return VisibilityToDisplay(isVisible);
    }

    protected ActionType[] GetFilteredSelectionActions(ActionType[] actionTypes)
    {
        return actionTypes
            .Where(x => x.IsSelectionAction())
            .ToArray();
    }

    protected ActionType[] GetFilteredDataActions(ActionType[] actionTypes)
    {
        return actionTypes
            .Where(x => x.IsDataAction())
            .ToArray();
    }

    protected ActionType[] GetFilteredLogicActions(ActionType[] actionTypes)
    {
        return actionTypes
            .Where(x => x.IsLogicAction())
            .ToArray();
    }

    protected ActionType[] GetFilteredInteractionActions(ActionType[] actionTypes)
    {
        return actionTypes
            .Where(x => x.IsInteractionAction())
            .ToArray();
    }

    protected string BuildActionToolip(ActionType actionType)
    {
        return actionType.BuildActionToolip();
    }

    protected string BuildActionIcon(ActionType actionType)
    {
        return actionType.BuildActionIcon();
    }

    protected Color BuildActionColor(ActionType actionType)
    {
        return actionType.BuildActionColor();
    }

    protected string BuildFriendlyName(ActionType actionType)
    {
        return actionType.BuildFriendlyName();
    }

    private string VisibilityToDisplay(bool isVisible)
    {
        return isVisible
            ? "block"
            : "none";
    }

    private void FilterActionsAsync(string filter)
    {
        var values = Enum.GetValues<ActionType>()
            .Where(x => x.ToString().Contains(filter, StringComparison.OrdinalIgnoreCase));

        FilteredActionTypes = values
            .ToArray();
    }

    private async Task HandleSelectActionTypeAsync(ActionType actionType)
    {
        await Task.Yield();
        SelectedActionType = actionType;
        Dialog?.Close(actionType);
    }
}
