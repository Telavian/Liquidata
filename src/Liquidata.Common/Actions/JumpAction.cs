using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class JumpAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Jump;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public Guid? JumpTargetId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; }    

    public override string[] BuildValidationErrors()
    {
        return JumpTargetId.IsNotDefined()
            ? (["No jump target selected"])
            : ([]);
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
