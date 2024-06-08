using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Microsoft.AspNetCore.Components;

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
