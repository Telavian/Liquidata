using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ForeachOptionsViewModel : ActionOptionsViewModelBase<ForeachAction>
{
    private ScriptType _scriptType;
    public ScriptType ScriptType
    {
        get => TypedAction.Script.GetMatchingScriptType();
        set => UpdateProperty(ref _scriptType, value,
            v =>
            {
                _scriptType = value;
                TypedAction.Script = v.BuildScript();
            });
    }
}
