using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Actions;

public class StoreAction : ActionBase
{
    public override ActionType ActionType => ActionType.Store;
    public override bool AllowChildren => false;

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
}
