using Liquidata.Client.Pages.Common;
using Liquidata.Client.Pages.Dialogs;
using Liquidata.Common.Actions.Shared;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class ActionOptionsViewModel : ViewModelBase
{    
    [Parameter]
    public ActionBase? Action { get; set; }

    [Parameter]
    public EditProjectViewModel? Parent { get; set; }

    private Func<Task>? _openOptionsInDialogAsyncCommand;
    public Func<Task> OpenOptionsInDialogAsyncCommand => _openOptionsInDialogAsyncCommand ??= CreateEventCallbackAsyncCommand(HandleOpenOptionsInDialogAsync, "Unable to open options");

    protected override async Task OnParametersSetAsync()
    {        
        await base.OnParametersSetAsync();

        // TODO: Optimize
        await RefreshAsync();
    }

    private async Task HandleOpenOptionsInDialogAsync()
    {
        await Task.Yield();

        var parameters = new DialogParameters
        {
            { nameof(ActionOptionsDialogViewModel.Action), Action },
            { nameof(ActionOptionsDialogViewModel.Parent), Parent }
        };

        await ShowDialogAsync<ActionOptionsDialog>("Options", parameters);
    }
}
