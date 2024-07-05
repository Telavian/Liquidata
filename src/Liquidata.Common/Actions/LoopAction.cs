using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class LoopAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Loop;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public int MaxTimesCount { get; set; } = 10;
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        var maxCount = MaxTimesCount <= 0
            ? 10
            : MaxTimesCount;

        for (var count = 0; count < maxCount; count++)
        {
            var returnType = await ExecuteChildrenAsync(executionService);

            if (returnType == ExecutionReturnType.StopLoop)
            {
                return ExecutionReturnType.Continue;
            }
            else if (returnType != ExecutionReturnType.Continue)
            {
                return returnType;
            }

            await WaitForDelayAsync(WaitMilliseconds);
        }

        return ExecutionReturnType.Continue;        
    }
}
