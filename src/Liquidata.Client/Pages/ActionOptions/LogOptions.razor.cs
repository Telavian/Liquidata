using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class LogOptionsViewModel : ActionOptionsViewModelBase<LogAction>
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

    public ExpressionType ExpressionType
    {
        get => TypedAction.ExpressionType;
        set
        {
            TypedAction.ExpressionType = value;
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
