using Liquidata.Common.Actions;

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
