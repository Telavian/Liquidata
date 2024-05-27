namespace Liquidata.Common.Actions;

public class ReloadAction : ActionBase
{
    public override ActionType ActionType => ActionType.Reload;
    public override bool AllowChildren => false;

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
