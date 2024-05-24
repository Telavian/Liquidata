namespace Liquidata.Common.Actions;

public class ScopeAction : ActionBase
{
    public override ActionType ActionType => ActionType.Scope;
    public override bool AllowChildren => true;
}
