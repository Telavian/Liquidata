namespace Liquidata.Common.Actions;

public class BeginRecordAction : ActionBase
{
    public override ActionType ActionType => ActionType.BeginRecord;
    public override bool AllowChildren => false;
}
