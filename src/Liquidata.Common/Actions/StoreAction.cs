using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;
using Liquidata.Common.Services.Interfaces;

namespace Liquidata.Common.Actions;

public class StoreAction : ActionBase
{
    [JsonIgnore] public override ActionType ActionType => ActionType.Store;
    [JsonIgnore] public override bool AllowChildren => false;
    [JsonIgnore] public override bool IsInteractive => false;

    public StoreType StoreType { get; set; }
    public ScriptType ScriptType { get; set; }
    public string? Script { get; set; } = null!;
    public FieldType FieldType { get; set; }

    public override string[] BuildValidationErrors()
    {
        var errors = new List<string>();

        if (Name.IsNotDefined())
        {
            errors.Add("Name is not defined");
        }

        if (Script.IsNotDefined())
        {
            errors.Add("Script is not defined");
        }

        return errors.ToArray();
    }

    public override async Task ExecuteActionAsync(IExecutionService service)
    {
        await Task.Yield();
    }
}
