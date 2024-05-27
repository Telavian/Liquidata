using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class JumpTargetAction : ActionBase
{
    public override ActionType ActionType => ActionType.JumpTarget;
    public override bool AllowChildren => false;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["Name is not defined"])
            : ([]);
    }
}
