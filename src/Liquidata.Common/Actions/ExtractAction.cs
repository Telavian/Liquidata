using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class ExtractAction : ActionBase
{
    public override ActionType ActionType => ActionType.Extract;
    public override bool AllowChildren => false;

    public ScriptType ScriptType { get; set; }    
    public string? Script { get; set; } = null!;
    public FieldType FieldType { get; set; }
}
