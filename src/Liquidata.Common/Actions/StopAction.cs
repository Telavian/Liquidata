using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class StopAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Stop;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await Task.Yield();

        if (StopType == StopType.Loop)
        {
            return ExecutionReturnType.StopLoop;
        }

        if (StopType == StopType.Template)
        {
            return ExecutionReturnType.StopTemplate;
        }

        if (StopType == StopType.Project)
        {
            return ExecutionReturnType.StopProject;
        }

        throw new ExecutionException($"Unknown stop type: {StopType}");
    }
}
