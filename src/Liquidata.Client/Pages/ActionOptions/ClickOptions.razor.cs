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

    public bool IsShift
    {
        get => TypedAction.IsShift;
        set
        {
            TypedAction.IsShift = value;
            _ = ActionUpdatedAsync();
        }
    }

    public bool IsCtrl
    {
        get => TypedAction.IsCtrl;
        set
        {
            TypedAction.IsCtrl = value;
            _ = ActionUpdatedAsync();
        }
    }

    public bool IsAlt
    {
        get => TypedAction.IsAlt;
        set
        {
            TypedAction.IsAlt = value;
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

        set
        {
            TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
            _ = ActionUpdatedAsync();
        }
    }
}
