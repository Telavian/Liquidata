using Liquidata.Common.Actions;
using Microsoft.AspNetCore.Components.Web;
using System.Text;

namespace Liquidata.Client.Pages.ActionOptions;

public partial class KeypressOptionsViewModel : ActionOptionsViewModelBase<KeypressAction>
{
    private const string ShiftKeyName = "Shift";
    private const string CtrlKeyName = "Ctrl";
    private const string ControlKeyName = "Control";
    private const string AltKeyName = "Alt";

    private Func<KeyboardEventArgs, Task>? _updateKeypressedAsyncCommand;
    public Func<KeyboardEventArgs, Task> UpdateKeypressedAsyncCommand => _updateKeypressedAsyncCommand ??= CreateEventCallbackAsyncCommand<KeyboardEventArgs>(HandleUpdateKeypressedAsync, "Unable to update keypressed");

    protected string BuildKeybordActionText()
    {
        var result = new StringBuilder();

        if (TypedAction.IsShiftPressed)
        {
            result.Append(ShiftKeyName);
            result.Append(" + ");
        }

        if (TypedAction.IsCtrlPressed)
        {
            result.Append(CtrlKeyName);
            result.Append(" + ");
        }

        if (TypedAction.IsAltPressed)
        {
            result.Append(AltKeyName);
            result.Append(" + ");
        }

        if (!string.IsNullOrWhiteSpace(TypedAction.Keypressed) &&
            !(TypedAction.IsShiftPressed && TypedAction.Keypressed == ShiftKeyName) &&
            !(TypedAction.IsCtrlPressed && (TypedAction.Keypressed == CtrlKeyName || TypedAction.Keypressed == ControlKeyName)) &&
            !(TypedAction.IsAltPressed && TypedAction.Keypressed == AltKeyName))
        {
            result.Append(TypedAction.Keypressed);
        }

        return result
            .ToString()
            .Trim(' ', '+');
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
