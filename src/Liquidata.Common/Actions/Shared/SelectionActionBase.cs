using Liquidata.Common.Actions.Shared;
using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions;

public abstract class SelectionActionBase : ActionBase
{    
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;

    public string? XPath { get; set; }
    public int WaitMilliseconds { get; set; }

    public override string[] BuildValidationErrors()
    {
        var errors = new List<string>();

        if (Name.IsNotDefined())
        {
            errors.Add("Name is not defined");
        }

        if (XPath.IsNotDefined())
        {
            errors.Add("Selection is not defined");
        }

        return errors.ToArray();
    }
}
