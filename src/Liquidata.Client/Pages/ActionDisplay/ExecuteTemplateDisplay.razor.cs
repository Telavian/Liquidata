﻿using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class ExecuteTemplateDisplayViewModel : ActionDisplayViewModelBase<ExecuteTemplateAction>
{
    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }
}
