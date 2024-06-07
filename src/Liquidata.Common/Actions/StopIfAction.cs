using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class StopIfAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.StopIf;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public string? Script { get; set; } = null!;
    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for stop if action");
        }

        var (isSuccess, result) = await executionService.Browser.ExecuteJavascriptAsync<bool>(Script!);

        if (!isSuccess)
        {
            throw new ExecutionException("Script not executed successfully for stop if action");
        }

        if (!result)
        {
            return ExecutionReturnType.Continue;
        }

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
