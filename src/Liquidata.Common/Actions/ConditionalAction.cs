using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class ConditionalAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Conditional;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for conditional action");
        }

        var (isSuccess, result) = await executionService.Browser.ExecuteJavascriptAsync<bool>(Script!);

        if (isSuccess && result) 
        {
            var returnType = await ExecuteChildrenAsync(executionService);

            if (returnType != ExecutionReturnType.Continue)
            {
                return returnType;
            }
        }

        return ExecutionReturnType.Continue;
    }
}
