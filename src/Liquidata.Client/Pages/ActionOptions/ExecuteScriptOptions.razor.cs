using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

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
