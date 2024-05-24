using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class ForeachAction : ActionBase
{
    public override ActionType ActionType => ActionType.Foreach;
    public override bool AllowChildren => true;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;    
}
