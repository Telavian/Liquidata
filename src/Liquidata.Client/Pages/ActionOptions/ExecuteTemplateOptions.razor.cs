using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ExecuteTemplateOptionsViewModel : ActionOptionsViewModelBase<ExecuteTemplateAction>
{
    public Template? ExecutionTemplate
    {
        get => Parent?.CurrentProject?.AllTemplates
            .FirstOrDefault(x => x.ActionId == TypedAction.ExecutionTemplateId);
        set => TypedAction.ExecutionTemplateId = value?.ActionId ?? null;
    }

    public int WaitMilliseconds
    {
        get => TypedAction.WaitMilliseconds;
        set
        {
            TypedAction.WaitMilliseconds = value;
            _ = ActionUpdatedAsync();
        }
    }
}
