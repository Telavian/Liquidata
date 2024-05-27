using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class InputAction : ActionBase
{
    public override ActionType ActionType => ActionType.Input;
    public override bool AllowChildren => false;

    public ScriptType ScriptType { get; set; }
    public ExpressionType ExpressionType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No input defined"])
            : ([]);
    }
}
