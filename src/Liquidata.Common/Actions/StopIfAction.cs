using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class StopIfAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.StopIf;
    [JsonIgnore] public override bool AllowChildren => false;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;
    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }
}
