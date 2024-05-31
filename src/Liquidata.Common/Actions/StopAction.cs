using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class StopAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Stop;
    [JsonIgnore] public override bool AllowChildren => false;

    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
