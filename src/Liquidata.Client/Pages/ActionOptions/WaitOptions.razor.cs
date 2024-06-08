using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class WaitOptionsViewModel : ActionOptionsViewModelBase<WaitAction>
{
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
