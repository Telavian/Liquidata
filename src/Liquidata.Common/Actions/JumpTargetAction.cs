using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class JumpTargetAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.JumpTarget;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["Name is not defined"])
            : ([]);
    }
}
