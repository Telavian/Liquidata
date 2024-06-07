using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class InputAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Input;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;

    public ExpressionType ExpressionType { get; set; }
    public string? Script { get; set; } = null!;
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No input defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for input action");
        }

        var inputValue = await EvaluateExpressionAsync(executionService, Script, ExpressionType)
            ?? "";

        await executionService.Browser.InputToSelectionAsync(executionService.CurrentSelection, inputValue);
        await WaitForDelayAsync(WaitMilliseconds);
        return ExecutionReturnType.Continue;
    }
}
