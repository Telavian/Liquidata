﻿using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class LoopOptionsViewModel : ActionOptionsViewModelBase<LoopAction>
{
    public int MaxTimesCount
    {
        get => TypedAction.MaxTimesCount;
        set
        {
            TypedAction.MaxTimesCount = value;
            _ = RefreshAsync();
        }
    }
}
