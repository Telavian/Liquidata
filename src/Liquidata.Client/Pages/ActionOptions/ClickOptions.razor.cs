using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ClickOptionsViewModel : ActionOptionsViewModelBase<ClickAction>
{
    public ClickType ClickType
    {
        get => TypedAction.ClickType;
        set
        {
            TypedAction.ClickType = value;
            RefreshAsync();
        }

    }

    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }
}
