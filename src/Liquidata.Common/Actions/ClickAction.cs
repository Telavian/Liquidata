﻿using Liquidata.Common.Actions.Enums;

namespace Liquidata.Common.Actions;

public class ClickAction : ActionBase
{
    public override ActionType ActionType => ActionType.Click;
    public override bool AllowChildren => false;

    public ClickType ClickType { get; set; } = ClickType.Continue;
    public Template? ExecutionTemplate { get; set; } = null!;
    public ClickButton ClickButton { get; set; } = ClickButton.Left;
    public bool IsDoubleClick { get; set; }
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; }
    public bool IsMaxTimesTemplate { get; set; }
}
