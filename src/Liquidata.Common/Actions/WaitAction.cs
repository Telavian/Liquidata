namespace Liquidata.Common.Actions;

public class WaitAction : ActionBase
{
    public override ActionType ActionType => ActionType.Wait;
    public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
