using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;
using System.Text.RegularExpressions;
using Liquidata.Common.Services;

namespace Liquidata.Common.Actions;

public class ForeachAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Foreach;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => true;

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
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

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
            await ExecuteItemAsync(executionService, item);
        }

        return ExecutionReturnType.Continue;
    }

    private async Task<ExecutionReturnType> ExecuteItemAsync(IExecutionService executionService, string item)
    {
        await Task.Yield();
        return await ExecuteSelectionActionAsync(executionService, item,
            async () =>
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
                return ExecutionReturnType.Continue;
            });
    }
}
