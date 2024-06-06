using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ScrollOptionsViewModel : ActionOptionsViewModelBase<ScrollAction>
{
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
