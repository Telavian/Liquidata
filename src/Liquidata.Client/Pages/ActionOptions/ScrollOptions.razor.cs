using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ScrollOptionsViewModel : ActionOptionsViewModelBase<ScrollAction>
{
    public ScrollType ScrollType
    {
        get => TypedAction.ScrollType;
        set
        {
            TypedAction.ScrollType = value;
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
}
