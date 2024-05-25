using Liquidata.Common.Actions;
using System.Text;

namespace Liquidata.Common.Extensions;

public static class KeypressActionExtensions
{
    private const string ShiftKeyName = "Shift";
    private const string CtrlKeyName = "Ctrl";
    private const string ControlKeyName = "Control";
    private const string AltKeyName = "Alt";

    public static string BuildKeypressText(this KeypressAction action)
    {
        var result = new StringBuilder();

        if (action.IsShiftPressed)
        {
            result.Append(ShiftKeyName);
            result.Append(" + ");
        }

        if (action.IsCtrlPressed)
        {
            result.Append(CtrlKeyName);
            result.Append(" + ");
        }

        if (action.IsAltPressed)
        {
            result.Append(AltKeyName);
            result.Append(" + ");
        }

        if (!string.IsNullOrWhiteSpace(action.Keypressed) &&
            !(action.IsShiftPressed && action.Keypressed == ShiftKeyName) &&
            !(action.IsCtrlPressed && (action.Keypressed == CtrlKeyName || action.Keypressed == ControlKeyName)) &&
            !(action.IsAltPressed && action.Keypressed == AltKeyName))
        {
            result.Append(action.Keypressed);
        }

        return result
            .ToString()
            .Trim(' ', '+');
    }
}
