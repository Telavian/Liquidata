using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class RelativeSelectOptionsViewModel : ActionOptionsViewModelBase<RelativeSelectAction>
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
