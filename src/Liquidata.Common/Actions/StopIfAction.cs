using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class StopIfAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.StopIf;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;
    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
