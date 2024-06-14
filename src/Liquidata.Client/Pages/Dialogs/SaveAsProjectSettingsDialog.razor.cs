using Liquidata.UI.Common.Pages.Dialogs;

namespace Liquidata.Client.Pages.Dialogs;

public class SaveAsProjectSettingsDialogViewModel : DialogViewModelBase
{
    public string Name { get; set; } = "";

    private Func<Task>? _saveAsyncCommand;
    public Func<Task> SaveAsyncCommand => _saveAsyncCommand ??= CreateEventCallbackAsyncCommand(() => HandleSaveAsync(), "Unable to save");

    private async Task HandleSaveAsync()
    {
        await Task.Yield();
        
        if (string.IsNullOrWhiteSpace(Name))
        {
            await ShowAlertAsync("Name is required");
            return;
        }

        Dialog?.Close(Name);
    }
}
