using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Liquidata.Common.Actions.Enums;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class ClickDisplayViewModel : ActionDisplayViewModelBase<ClickAction>
{
    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }

    protected string BuildActionText()
    {
        if (TypedAction.ClickType == ClickType.Continue)
        {
            return "Click and continue";
        }
        else if (TypedAction.ClickType == ClickType.ExecuteTemplate)
        {
            return $"Click and run {ExecutionTemplate?.Name}";
        }

        return "";
    }
}
