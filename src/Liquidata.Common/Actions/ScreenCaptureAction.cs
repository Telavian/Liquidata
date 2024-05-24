namespace Liquidata.Common.Actions;

public class ScreenCaptureAction : ActionBase
{
    public override ActionType ActionType => ActionType.ScreenCapture;
    public override bool AllowChildren => false;
}
