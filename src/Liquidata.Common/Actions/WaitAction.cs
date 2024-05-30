using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class WaitAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Wait;
    [JsonIgnore] public override bool AllowChildren => false;

    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
