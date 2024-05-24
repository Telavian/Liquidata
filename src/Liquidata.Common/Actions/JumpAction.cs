using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class JumpAction : ActionBase
{
    public override ActionType ActionType => ActionType.Jump;
    public override bool AllowChildren => false;

    public Guid? JumpTargetId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;
}
