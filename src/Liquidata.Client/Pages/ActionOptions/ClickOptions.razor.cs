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
            _ = ActionUpdatedAsync();
        }
    }

    public ClickButton ClickButton
    {
        get => TypedAction.ClickButton;
        set
        {
            TypedAction.ClickButton = value;
            _ = ActionUpdatedAsync();
        }
    }

    public bool IsDoubleClick
    {
        get => TypedAction.IsDoubleClick;
        set
        {
            TypedAction.IsDoubleClick = value;
            _ = ActionUpdatedAsync();
        }
    }

    public bool IsNewPage
    {
        get => TypedAction.IsNewPage;
        set
        {
            TypedAction.IsNewPage = value;
            _ = ActionUpdatedAsync();
        }
    }

    public int WaitMilliseconds
    {
        get => TypedAction.WaitMilliseconds;
        set
        {
            TypedAction.WaitMilliseconds = value;
            _ = ActionUpdatedAsync();
        }
    }

    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }
}
