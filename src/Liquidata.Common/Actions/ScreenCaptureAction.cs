﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class ScreenCaptureAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.ScreenCapture;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }
}
