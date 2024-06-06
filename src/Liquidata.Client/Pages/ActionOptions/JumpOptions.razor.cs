using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class JumpOptionsViewModel : ActionOptionsViewModelBase<JumpAction>
{
    public JumpTargetAction? JumpTarget
    {
        get => Parent?.SelectedTemplate
            ?.FindActions(x => x.ActionId == TypedAction.JumpTargetId)
            ?.OfType<JumpTargetAction>()
            ?.FirstOrDefault();
        set => TypedAction.JumpTargetId = value?.ActionId ?? null;
    }

    public int MaxTimesCount
    {
        get => TypedAction.MaxTimesCount;
        set
        {
            TypedAction.MaxTimesCount = value;
            RefreshAsync();
        }
    }
}
