using Liquidata.Common;
using Liquidata.UI.Common.Pages.Dialogs;
using Microsoft.AspNetCore.Components;

namespace Liquidata.Client.Pages.Dialogs;

public class ProjectSettingsDialogViewModel : DialogViewModelBase
{
    [Parameter]
    public Project CurrentProject { get; set; } = null!;

    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
    public int Concurrency { get; set; }

    private Func<Task>? _saveSettingsAsyncCommand;
    public Func<Task> SaveSettingsAsyncCommand => _saveSettingsAsyncCommand ??= CreateEventCallbackAsyncCommand(() => HandleSaveSettingsAsync(), "Unable to save settings");

    protected override async Task OnParametersSetAsync()
    {
        await Task.Yield();

        Name = CurrentProject?.Name ?? "";
        Url = CurrentProject?.Url ?? "";
        Concurrency = CurrentProject?.Concurrency ?? 0;
    }

    private async Task HandleSaveSettingsAsync()
    {
        await Task.Yield();
        
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowAlertAsync("Name is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(Url))
        {
            await ShowAlertAsync("Url is required");
            return;
        }

        CurrentProject.Name = Name;
        CurrentProject.Url = Url;
        CurrentProject.Concurrency = Concurrency;

        Dialog?.Close();
    }
}
