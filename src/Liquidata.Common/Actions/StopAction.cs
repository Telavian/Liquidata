using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class StopAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Stop;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public StopType StopType { get; set; }

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
