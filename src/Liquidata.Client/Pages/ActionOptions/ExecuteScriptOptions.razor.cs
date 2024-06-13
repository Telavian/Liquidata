using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ExecuteScriptOptionsViewModel : ActionOptionsViewModelBase<ExecuteScriptAction>
{
    public string? Script
    {
        get => TypedAction.Script;
        set
        {
            TypedAction.Script = value;
            _ = ActionUpdatedAsync();
        }
    }
}
