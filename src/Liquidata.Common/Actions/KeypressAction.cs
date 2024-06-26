﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class KeypressAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Keypress;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;
    [JsonIgnore] public override bool IsNameRequired => false;

    public bool IsShiftPressed { get; set; }
    public bool IsCtrlPressed { get; set; }
    public bool IsAltPressed { get; set; }
    public string Keypressed { get; set; } = "";
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return Keypressed.IsNotDefined()
            ? (["No keypress selected"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        if (Keypressed.IsNotDefined())
        {
            throw new ExecutionException("Key pressed is not defined for keypress action");
        }

        await executionService.Browser.KeypressToSelectionAsync(executionService.CurrentSelection, IsShiftPressed, IsCtrlPressed, IsAltPressed, Keypressed);
        await WaitForDelayAsync(WaitMilliseconds);
        return ExecutionReturnType.Continue;
    }
}
