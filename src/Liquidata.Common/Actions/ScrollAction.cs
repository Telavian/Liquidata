using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class ScrollAction : ActionBase
{
    public override ActionType ActionType => ActionType.Scroll;
    public override bool AllowChildren => false;

    public ScrollType ScrollType { get; set; }
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
