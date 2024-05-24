namespace Liquidata.Common.Actions;

public class HoverAction : ActionBase
{
    public override ActionType ActionType => ActionType.Hover;
    public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }
}
