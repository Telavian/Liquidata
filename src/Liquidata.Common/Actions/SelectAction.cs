using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Services.Interfaces;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public class SelectAction : SelectionActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Select;

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
