using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ConditionalAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Conditional;
    [JsonIgnore] public override bool AllowChildren => true;

    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }
}
