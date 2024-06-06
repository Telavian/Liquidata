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
    
    public string? Script { get; set; } = null!;

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
            throw new ExecutionException("Script is not defined for conditional action");
        }

        var (isSuccess, result) = await executionService.Browser.ExecuteScriptAsync<bool>(Script);

        if (isSuccess && result) 
        {
            await ExecuteChildrenAsync(executionService);
        }
    }
}
