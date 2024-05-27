using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class SelectAction : ActionBase
{
    public override ActionType ActionType => ActionType.Select;
    public override bool AllowChildren => true;

    public string? XPath { get; set; }
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return XPath.IsNotDefined()
            ? (["No selection defined"])
            : ([]);
    }
}
