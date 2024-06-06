using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Exceptions;
using Liquidata.Common.Extensions;
using Liquidata.Common.Services.Interfaces;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class RelativeSelectAction : SelectionActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.RelativeSelect;

    public override async Task<ExecutionReturnType> ExecuteActionAsync(IExecutionService executionService)
    {
        var selectionXPath = executionService.XPathProcessor
            .MakeRelativeXPathQuery(executionService.CurrentSelection, XPath);

        if (selectionXPath.IsNotDefined())
        {
            throw new ExecutionException("Relative selection is not defined");
        }

        var matches = await executionService.Browser.GetAllMatchesAsync(selectionXPath!);

        foreach (var match in matches)
        {
            try
            {
                await executionService.Browser.SetVariableAsync(Name, match);
                await ExecuteChildrenAsync(executionService);
                await WaitForDelayAsync(WaitMilliseconds);
            }
            finally
            {
                await executionService.Browser.RemoveVariableAsync(Name);
            }
        }
    }
}
