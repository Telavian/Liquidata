using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class LogAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Log;
    [JsonIgnore] public override bool AllowChildren => false;

    public ScriptType ScriptType { get; set; }
    public ExpressionType ExpressionType { get; set; }
    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No message defined"])
            : ([]);
    }
}
