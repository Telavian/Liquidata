using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class SelectOptionsViewModel : ActionOptionsViewModelBase<SelectAction>
{
    public string? XPath
    {
        get => TypedAction.XPath;
        set
        {
            TypedAction.XPath = value;
            _ = ActionUpdatedAsync();
        }
    }

    public int ItemWaitMilliseconds
    {
        get => TypedAction.ItemWaitMilliseconds;
        set
        {
            TypedAction.ItemWaitMilliseconds = value;
            _ = ActionUpdatedAsync();
        }
    }

    public int SelectionWaitMilliseconds
    {
        get => TypedAction.SelectionWaitMilliseconds;
        set
        {
            TypedAction.SelectionWaitMilliseconds = value;
            _ = ActionUpdatedAsync();
        }
    }
}
