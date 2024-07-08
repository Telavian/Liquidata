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
        if (IsDisabled)
        {
            return ExecutionReturnType.Continue;
        }

        var selectionXPath = executionService.XPathProcessor
            .MakeRelativeXPathQuery(executionService.CurrentSelection, XPath);

        if (selectionXPath.IsNotDefined())
        {
            throw new ExecutionException("Relative selection is not defined");
        }

        var matches = await executionService.Browser.GetAllMatchesAsync(selectionXPath!, SelectionWaitMilliseconds);

        foreach (var match in matches)
        {
            var returnType = await ExecuteSelectionActionAsync(executionService, match,
                async () =>
                {
                    var returnType = await ExecuteChildrenAsync(executionService);

                    if (returnType == ExecutionReturnType.StopLoop)
                    {
                        return ExecutionReturnType.Continue;
                    }
                    else if (returnType != ExecutionReturnType.Continue)
                    {
                        return returnType;
                    }

                    await WaitForDelayAsync(ItemWaitMilliseconds);
                    return ExecutionReturnType.Continue;
                });


            if (returnType != ExecutionReturnType.Continue)
            {
                return returnType;
            }
        }

        return ExecutionReturnType.Continue;
    }
}
