using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class KeypressAction : ActionBase
{
    public override ActionType ActionType => ActionType.Keypress;
    public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; }
    public bool IsMaxTimesTemplate { get; set; }
}
