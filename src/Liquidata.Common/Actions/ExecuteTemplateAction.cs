using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class ExecuteTemplateAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.ExecuteTemplate;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public Guid? ExecutionTemplateId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return ExecutionTemplateId.IsNotDefined()
            ? (["No template selected"])
            : ([]);
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
