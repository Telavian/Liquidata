using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class StoreAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Store;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => true;

    public StoreType StoreType { get; set; }
    public string? Script { get; set; } = null!;
    public FieldType FieldType { get; set; }

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

        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for store action");
        }

        if (Name.IsNotDefined())
        {
            throw new ExecutionException("Name is not defined for store action");
        }

        var (isSuccess, result) = await executionService.Browser.ExecuteJavascriptAsync<string>(Script!);

        if (!isSuccess)
        {
            throw new ExecutionException("Script not executed successfully for extract action");
        }

        result = await executionService.DataHandler.CleanDataAsync(result, FieldType);
        await executionService.Browser.StoreDataAsync(Name, result, StoreType);
        return ExecutionReturnType.Continue;
    }
}
