using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ConditionalOptionsViewModel : ActionOptionsViewModelBase<ConditionalAction>
{
    public ScriptType ScriptType
    {
        get => TypedAction.Script.GetMatchingScriptType();
        set
        {            
            TypedAction.Script = value.BuildScript();
            _ = ActionUpdatedAsync();
        }
    }

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
