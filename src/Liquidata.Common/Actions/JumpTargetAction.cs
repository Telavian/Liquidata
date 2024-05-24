namespace Liquidata.Common.Actions;

public class JumpTargetAction : ActionBase
{
    public override ActionType ActionType => ActionType.JumpTarget;
    public override bool AllowChildren => false;
}
