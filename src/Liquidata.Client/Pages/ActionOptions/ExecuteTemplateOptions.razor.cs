using Liquidata.Client.Pages.Common;
using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ExecuteTemplateOptionsViewModel : ActionOptionsViewModelBase<ExecuteTemplateAction>
{
    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }
}
