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

    public int MaxTimesCount { get; set; }
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

        var count = 0;

        var maxCount = MaxTimesCount <= 0
            ? int.MaxValue
            : MaxTimesCount;

        foreach (var child in ChildActions)
        {
            if (count >= maxCount)
            {
                return ExecutionReturnType.Continue;
            }

            var returnType = await child.ExecuteActionAsync(executionService);

            if (returnType == ExecutionReturnType.StopLoop)
            {
                return ExecutionReturnType.Continue;
            }
            else if (returnType != ExecutionReturnType.Continue)
            {
                return returnType;
            }

            count++;
            await WaitForDelayAsync(WaitMilliseconds);
        }

        return ExecutionReturnType.Continue;
    }
}
