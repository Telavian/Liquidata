using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class ScreenCaptureAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.ScreenCapture;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (Name.IsNotDefined())
        {
            throw new ExecutionException("Name is not defined for screen capture action");
        }

        var screenshot = await executionService.Browser.GetScreenshotAsync();
        await executionService.SaveScreenshotAsync(Name, screenshot);
    }
}
