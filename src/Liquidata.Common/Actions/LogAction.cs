using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class LogAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Log;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public ScriptType ScriptType { get; set; }
    public ExpressionType ExpressionType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No message defined"])
            : ([]);
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
