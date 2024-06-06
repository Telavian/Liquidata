using Liquidata.Common.Actions.Shared;
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

    public override string[] BuildValidationErrors()
    {
        return Name.IsNotDefined()
            ? (["No name defined"])
            : ([]);
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
