﻿using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Services;

namespace Liquidata.Common.Actions;

public class ClickAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Click;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => true;
    [JsonIgnore] public override bool IsNameRequired => false;

    public ClickType ClickType { get; set; } = ClickType.Continue;
    public bool IsNewPage { get; set; }
    public Guid? ExecutionTemplateId { get; set; } = null!;
    public ClickButton ClickButton { get; set; } = ClickButton.Left;
    public bool IsDoubleClick { get; set; }
    public bool IsShift { get; set; }
    public bool IsCtrl { get; set; }
    public bool IsAlt { get; set; }
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return ClickType == ClickType.ExecuteTemplate && ExecutionTemplateId.IsNotDefined() 
            ? (["No template selected"]) 
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        var isSelectionDisabled = await executionService.Browser
            .CheckSelectionDisabledAsync(executionService.CurrentSelection);

        if (IsDisabled || isSelectionDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        if (IsNewPage)
        {
            // New pages must execute in new template
            var newTemplate = executionService.Project.AllTemplates
                .FirstOrDefault(x => x.ActionId == ExecutionTemplateId) 
                ?? throw new ExecutionException("Unable to find template for click action");

            var selection = executionService.CurrentSelection;
            await executionService.CreateExecutionTaskAsync(async () =>
            {
                var childDataHandler = executionService.DataHandler.Clone();                

                var browser = await executionService.Browser.ClickOpenInNewPageAsync(selection, ClickButton, IsDoubleClick);
                var childExecutionService = executionService.Clone(selection:"", browser:browser, dataHandler:childDataHandler);
                await childExecutionService.RegisterBrowserAsync(browser);

                try
                {
                    await WaitForDelayAsync(WaitMilliseconds);
                    await newTemplate.ExecuteActionAsync(childExecutionService);
                }                
                finally
                {
                    await childExecutionService.UnregisterBrowserAsync(browser);
                    await executionService.DataHandler.MergeDataAsync(childDataHandler);
                }
            });

            return ExecutionReturnType.Continue;
        }

        await executionService.Browser.ClickSelectionAsync(executionService.CurrentSelection, ClickButton, IsDoubleClick, IsShift, IsCtrl, IsAlt);
        await WaitForDelayAsync(WaitMilliseconds);

        if (ClickType == ClickType.ExecuteTemplate)
        {
            var newTemplate = executionService.Project.AllTemplates
                .FirstOrDefault(x => x.ActionId == ExecutionTemplateId)
                ?? throw new ExecutionException("Unable to find template for click action");

            await newTemplate.ExecuteActionAsync(executionService);
        }

        return ExecutionReturnType.Continue;
    }
}
