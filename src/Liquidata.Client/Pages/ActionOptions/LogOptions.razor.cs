using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class LogOptionsViewModel : ActionOptionsViewModelBase<LogAction>
{
    private ScriptType _scriptType;
    public ScriptType ScriptType
    {
        get => TypedAction.ScriptType;
        set => UpdateProperty(ref _scriptType, value,
            v =>
            {
                TypedAction.ScriptType = value;
                TypedAction.Script = v.BuildScript();
            });
    }
}
