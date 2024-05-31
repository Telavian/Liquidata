using Liquidata.Common.Actions.Enums;

namespace Liquidata.Client.Pages.Dialogs;

public class SelectionOperationDialogViewModel : DialogViewModelBase
{
    private Func<SelectionOperation, Task>? _saveSelectionOperationAsyncCommand;
    public Func<SelectionOperation, Task> SaveSelectionOperationAsyncCommand => _saveSelectionOperationAsyncCommand ??= CreateEventCallbackAsyncCommand<SelectionOperation>(HandleSaveSelectionOperationAsync, "Unable to save selection operation");

    private async Task HandleSaveSelectionOperationAsync(SelectionOperation operation)
    {
        await Task.Yield();
        Dialog?.Close(operation);
    }
}
