﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class ScopeAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Scope;
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => true;

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["Name is not defined"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        var currentScope = executionService.DataHandler.DataScope;

        try
        {            
            executionService.DataHandler.DataScope = Name;
            var returnType = await ExecuteChildrenAsync(executionService);

            if (returnType == ExecutionReturnType.StopLoop)
            {
                return ExecutionReturnType.Continue;
            }
            else if (returnType != ExecutionReturnType.Continue)
            {
                return returnType;
            }

            return ExecutionReturnType.Continue;
        }
        finally
        {
            executionService.DataHandler.DataScope = currentScope;
        }
    }
}
