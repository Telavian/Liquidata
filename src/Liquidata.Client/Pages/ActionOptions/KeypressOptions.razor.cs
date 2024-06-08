using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class KeypressOptionsViewModel : ActionOptionsViewModelBase<KeypressAction>
{
    private Func<KeyboardEventArgs, Task>? _updateKeypressedAsyncCommand;
    public Func<KeyboardEventArgs, Task> UpdateKeypressedAsyncCommand => _updateKeypressedAsyncCommand ??= CreateEventCallbackAsyncCommand<KeyboardEventArgs>(HandleUpdateKeypressedAsync, "Unable to update keypressed");

    public int WaitMilliseconds
    {
        get => TypedAction.WaitMilliseconds;
        set
        {
            TypedAction.WaitMilliseconds = value;
            _ = ActionUpdatedAsync();
        }
    }

    private async Task HandleUpdateKeypressedAsync(KeyboardEventArgs args)
    {
        await Task.Yield();

        TypedAction.IsShiftPressed = args.ShiftKey;
        TypedAction.IsCtrlPressed = args.CtrlKey;
        TypedAction.IsAltPressed = args.AltKey;
        TypedAction.Keypressed = args.Key;        
    }
}
