using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class ExecuteTemplateAction : ActionBase
{
    public override ActionType ActionType => ActionType.ExecuteTemplate;
    public override bool AllowChildren => false;

    public Template? ExecutionTemplate { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;
}
