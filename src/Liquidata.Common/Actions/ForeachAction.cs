using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class ForeachAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Foreach;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;

    public string? Script { get; set; } = null!;
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        var errors = new List<string>();

        if (Name.IsNotDefined())
        {
            errors.Add("Name is not defined");
        }

        if (Script.IsNotDefined())
        {
            errors.Add("Script is not defined");
        }

        return errors.ToArray();
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (Name.IsNotDefined())
        {
            throw new ExecutionException("Name is not defined for foreach action");
        }

        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for foreach action");
        }

        var (isSuccess, result) = await executionService.Browser.ExecuteJavascriptAsync<string[]>(Script!);

        if (!isSuccess)
        {
            throw new ExecutionException("Script not executed successfully for foreach action");
        }

        foreach (var item in result) 
        {
            try
            {
                await executionService.Browser.SetVariableAsync(Name, item);
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
            finally
            {
                await executionService.Browser.RemoveVariableAsync(Name);
            }
        }

        return ExecutionReturnType.Continue;
    }
}
