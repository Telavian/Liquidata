using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class ConditionalAction : ActionBase
{
    public override ActionType ActionType => ActionType.Conditional;
    public override bool AllowChildren => true;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }
}
