using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class StopIfAction : ActionBase
{
    public override ActionType ActionType => ActionType.StopIf;
    public override bool AllowChildren => false;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;
    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }
}
