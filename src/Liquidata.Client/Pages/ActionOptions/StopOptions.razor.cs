using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class StopOptionsViewModel : ActionOptionsViewModelBase<StopAction>
{
    public StopType StopType
    {
        get => TypedAction.StopType;
        set
        {
            TypedAction.StopType = value;
            _ = ActionUpdatedAsync();
        }
    }
}
