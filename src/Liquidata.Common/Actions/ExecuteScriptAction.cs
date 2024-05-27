using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class ExecuteScriptAction : ActionBase
{
    public override ActionType ActionType => ActionType.ExecuteScript;
    public override bool AllowChildren => false;

    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }
}
