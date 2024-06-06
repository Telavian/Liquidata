using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class Template : ActionBase
{
    public const string MainTemplateName = "main";

    [JsonIgnore] public override ActionType ActionType => ActionType.Template;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;

    public string Url { get; set; } = "";

    public override string[] BuildValidationErrors()
    {
        // TODO: Url required?
        return [];
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        await Task.Yield();
        await ExecuteChildrenAsync(executionService);
        return ExecutionReturnType.Continue;
    }
}
