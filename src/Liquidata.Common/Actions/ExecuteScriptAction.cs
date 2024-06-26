﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class ExecuteScriptAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.ExecuteScript;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;
    [JsonIgnore] public override bool IsNameRequired => false;

    public string? Script { get; set; } = null!;

    public override string[] BuildValidationErrors()
    {
        return Script.IsNotDefined()
            ? (["No script defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        if (Script.IsNotDefined())
        {
            throw new ExecutionException("Script is not defined for execute script action");
        }

        var isSuccess = await executionService.Browser.ExecuteJavascriptAsync(Script!);

        if (!isSuccess)
        {
            throw new ExecutionException("Script not executed successfully for execute script action");
        }

        return ExecutionReturnType.Continue;
    }
}
