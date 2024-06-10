using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;
using Liquidata.Common.Exceptions;

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
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        return ClickType == ClickType.ExecuteTemplate && ExecutionTemplateId.IsNotDefined() 
            ? (["No template selected"]) 
            : ([]);
    }

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        if (IsNewPage)
        {
            // New pages must execute in new template
            var newTemplate = executionService.CurrentProject.AllTemplates
                .FirstOrDefault(x => x.ActionId == ExecutionTemplateId) 
                ?? throw new ExecutionException("Unable to find template for click action");

            var selection = executionService.CurrentSelection;
            await executionService.CreateExecutionTaskAsync(async () =>
            {
                var browser = await executionService.Browser.ClickOpenInNewPageAsync(selection, ClickButton, IsDoubleClick);
                executionService = executionService.Clone(selection:"", browser:browser);

                await WaitForDelayAsync(WaitMilliseconds);
                await newTemplate.ExecuteActionAsync(executionService);
            });

            return ExecutionReturnType.Continue;
        }

        await executionService.Browser.ClickSelectionAsync(executionService.CurrentSelection, ClickButton, IsDoubleClick);
        await WaitForDelayAsync(WaitMilliseconds);

        if (ClickType == ClickType.ExecuteTemplate)
        {
            var newTemplate = executionService.CurrentProject.AllTemplates
                .FirstOrDefault(x => x.ActionId == ExecutionTemplateId)
                ?? throw new ExecutionException("Unable to find template for click action");

            await newTemplate.ExecuteActionAsync(executionService);
        }

        return ExecutionReturnType.Continue;
    }
}
