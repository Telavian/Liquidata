using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class ClickDisplayViewModel : ActionDisplayViewModelBase<ClickAction>
{
    protected string BuildActionText()
    {
        if (TypedAction.ClickType == ClickType.Continue)
        {
            return "Click and continue";
        }
        else if (TypedAction.ClickType == ClickType.ExecuteTemplate)
        {
            return $"Click and run {TypedAction?.ExecutionTemplate?.Name}";
        }

        return "";
    }
}
