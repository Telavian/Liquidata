using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class ExecuteTemplateAction : ActionBase
{
    public override ActionType ActionType => ActionType.ExecuteTemplate;
    public override bool AllowChildren => false;

    public Guid? ExecutionTemplateId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return ExecutionTemplateId.IsNotDefined()
            ? (["No template selected"])
            : ([]);
    }
}
