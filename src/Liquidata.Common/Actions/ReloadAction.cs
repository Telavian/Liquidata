using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class ReloadAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Reload;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;

    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await executionService.Browser.ReloadPageAsync();
        await WaitForDelayAsync(WaitMilliseconds);

        return ExecutionReturnType.Continue;
    }
}
