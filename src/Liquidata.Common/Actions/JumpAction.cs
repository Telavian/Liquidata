using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class JumpAction : ActionBase
{
    public override ActionType ActionType => ActionType.Jump;
    public override bool AllowChildren => false;

    public ActionBase? JumpTarget { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; }
    public bool IsMaxTimesTemplate { get; set; }
}
