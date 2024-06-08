using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class TemplateOptionsViewModel : ActionOptionsViewModelBase<Template>
{
    public string? Url
    {
        get => TypedAction.Url;
        set
        {
            TypedAction.Url = value;
            _ = ActionUpdatedAsync();
        }
    }
}
