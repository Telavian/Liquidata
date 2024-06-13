using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ReloadOptionsViewModel : ActionOptionsViewModelBase<ReloadAction>
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
