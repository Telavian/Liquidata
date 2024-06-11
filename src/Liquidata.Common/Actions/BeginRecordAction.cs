using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class BeginRecordAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.BeginRecord;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await Task.Yield();
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        executionService.DataHandler
            .AddRecord();

        return ExecutionReturnType.Continue;
    }
}
