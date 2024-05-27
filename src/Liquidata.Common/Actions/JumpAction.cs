using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class JumpAction : ActionBase
{
    public override ActionType ActionType => ActionType.Jump;
    public override bool AllowChildren => false;

    public Guid? JumpTargetId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return JumpTargetId.IsNotDefined()
            ? (["No jump target selected"])
            : ([]);
    }
}
