using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class ScrollAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Scroll;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public ScrollType ScrollType { get; set; }
    public int WaitMilliseconds { get; set; }
    public int MaxTimesCount { get; set; }    

    public override string[] BuildValidationErrors()
    {
        return [];
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
