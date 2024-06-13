using Liquidata.Common.Actions;

namespace Liquidata.Client.Pages.ActionDisplay;

public partial class TemplateDisplayViewModel : ActionDisplayViewModelBase<Template>
{
    private Func<Task>? _loadTemplateUrlAsyncCommand;
    public Func<Task> LoadTemplateUrlAsyncCommand => _loadTemplateUrlAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleLoadTemplateUrlAsync, "Unable to load template url");

    private async Task HandleLoadTemplateUrlAsync()
    {
        await Task.Yield();
        await Parent!.LoadTemplateUrlAsyncCommand(TypedAction.Url);
    }
}
