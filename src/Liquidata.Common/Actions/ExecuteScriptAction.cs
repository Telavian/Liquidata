namespace Liquidata.Common.Actions;

public class ExecuteScriptAction : ActionBase
{
    public override ActionType ActionType => ActionType.ExecuteScript;
    public override bool AllowChildren => false;

    public string? Script { get; set; } = null!;
}
