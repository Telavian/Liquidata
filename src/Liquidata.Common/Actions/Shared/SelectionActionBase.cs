using Liquidata.Common.Extensions;
using System.Text.Json.Serialization;

namespace Liquidata.Common.Actions.Shared;

public abstract class SelectionActionBase : ActionBase
{
    [JsonIgnore] public override bool AllowChildren => true;
    [JsonIgnore] public override bool IsInteractive => false;
    [JsonIgnore] public override bool IsNameRequired => true;

    public string? XPath { get; set; }
    public int ItemWaitMilliseconds { get; set; }
    public int SelectionWaitMilliseconds { get; set; }

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
