using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class JumpDisplayViewModel : ActionDisplayViewModelBase<JumpAction>
{
    public JumpTargetAction? JumpTarget
    {
        get => Parent?.SelectedTemplate
            ?.FindActions(x => x.ActionId == TypedAction.JumpTargetId)
            ?.OfType<JumpTargetAction>()
            ?.FirstOrDefault();
        set => TypedAction.JumpTargetId = value?.ActionId ?? null;
    }
}
