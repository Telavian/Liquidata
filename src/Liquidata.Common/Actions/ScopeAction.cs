using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class ScopeAction : ActionBase
{
    public override ActionType ActionType => ActionType.Scope;
    public override bool AllowChildren => true;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }
}
