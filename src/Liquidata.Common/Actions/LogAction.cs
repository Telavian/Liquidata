using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class LogAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Log;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public ExpressionType ExpressionType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No message defined"])
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
            throw new ExecutionException("Script is not defined for input action");
        }

        var message = await EvaluateExpressionAsync(executionService, Script, ExpressionType)
            ?? "";
        await executionService.LogMessageAsync(message);
        return ExecutionReturnType.Continue;
    }
}
