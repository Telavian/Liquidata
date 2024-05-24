using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class StoreAction : ActionBase
{
    public override ActionType ActionType => ActionType.Store;
    public override bool AllowChildren => false;

    public StoreType StoreType { get; set; }
    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;
    public FieldType FieldType { get; set; }
}
