using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class StopAction : ActionBase
{
    public override ActionType ActionType => ActionType.Stop;
    public override bool AllowChildren => false;

    public StopType StopType { get; set; }
}
