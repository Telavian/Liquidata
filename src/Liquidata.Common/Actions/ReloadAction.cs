using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ReloadAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Reload;
    [JsonIgnore] public override bool AllowChildren => false;

    public override string[] BuildValidationErrors()
    {
        return [];
    }
}
