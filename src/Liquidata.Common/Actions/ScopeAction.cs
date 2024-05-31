using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ScopeAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Scope;
    [JsonIgnore] public override bool AllowChildren => true;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }
}
