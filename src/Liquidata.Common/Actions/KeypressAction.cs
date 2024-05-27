using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class KeypressAction : ActionBase
{
    public override ActionType ActionType => ActionType.Keypress;
    public override bool AllowChildren => false;

    public bool IsShiftPressed { get; set; }
    public bool IsCtrlPressed { get; set; }
    public bool IsAltPressed { get; set; }
    public string Keypressed { get; set; } = "";
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; } = 1;
    public bool IsMaxTimesTemplate { get; set; } = true;

    public override string[] BuildValidationErrors()
    {
        return Keypressed.IsNotDefined()
            ? (["No keypress selected"])
            : ([]);
    }
}
