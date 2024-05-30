using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class RelativeSelectAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.RelativeSelect;
    [JsonIgnore] public override bool AllowChildren => true;

    public string? XPath { get; set; }
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return XPath.IsNotDefined()
            ? (["No selection defined"])
            : ([]);
    }
}
