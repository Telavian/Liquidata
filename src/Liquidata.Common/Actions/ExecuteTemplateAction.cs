﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

namespace Liquidata.Common.Actions;

public class ExecuteTemplateAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.ExecuteTemplate;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => false;

    public Guid? ExecutionTemplateId { get; set; } = null!;
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return ExecutionTemplateId.IsNotDefined()
            ? (["No template selected"])
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        var newTemplate = executionService.Project.AllTemplates
            .FirstOrDefault(x => x.ActionId == ExecutionTemplateId)
            ?? throw new ExecutionException("Unable to find template to execute");

        var returnType = await newTemplate.ExecuteActionAsync(executionService);
        await WaitForDelayAsync(WaitMilliseconds);

        return returnType;
    }
}
