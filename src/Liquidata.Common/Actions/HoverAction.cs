using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class HoverAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Hover;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;
    [JsonIgnore] public override bool IsNameRequired => false;

    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await executionService.Browser.HoverSelectionAsync(executionService.CurrentSelection);
        await WaitForDelayAsync(WaitMilliseconds);
        return ExecutionReturnType.Continue;
    }
}
