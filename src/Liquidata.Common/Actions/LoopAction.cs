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
    
    public int MaxTimesCount { get; set; }
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        var count = 0;

        var maxCount = MaxTimesCount <= 0
            ? int.MaxValue
            : MaxTimesCount;

        foreach (var child in ChildActions)
        {
            if (count >= maxCount)
            {
                return;
            }

            await child.ExecuteActionAsync(executionService);
            await WaitForDelayAsync(WaitMilliseconds);
        }
    }
}
