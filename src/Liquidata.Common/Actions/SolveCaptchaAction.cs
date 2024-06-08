using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class SolveCaptchaAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.SolveCaptcha;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await executionService.Browser.SolveCaptchaAsync();
        await WaitForDelayAsync(WaitMilliseconds);
        return ExecutionReturnType.Continue;
    }
}
