using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class ScreenCaptureAction : ActionBase
{
    public override ActionType ActionType => ActionType.ScreenCapture;
    public override bool AllowChildren => false;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }
}
