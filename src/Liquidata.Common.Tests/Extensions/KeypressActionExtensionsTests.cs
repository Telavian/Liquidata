using Liquidata.Common.Actions;
using Liquidata.Common.Extensions;

namespace Liquidata.Common.Tests.Extensions;

public class KeypressActionExtensionsTests
{
    [Theory]
    [InlineData(false, false, false, "X", "X")]
    [InlineData(false, false, true, "X", "Alt + X")]
    [InlineData(false, true, false, "X", "Ctrl + X")]
    [InlineData(false, true, true, "X", "Ctrl + Alt + X")]
    [InlineData(true, false, false, "X", "Shift + X")]
    [InlineData(true, false, true, "X", "Shift + Alt + X")]
    [InlineData(true, true, false, "X", "Shift + Ctrl + X")]
    [InlineData(true, true, true, "X", "Shift + Ctrl + Alt + X")]
    public void GivenKeypress_WhenCalled_ThenCorrect(bool isShift, bool isCtrl, bool isAlt, string key, string expected)
    {
        var keypress = new KeypressAction 
        { 
            IsShiftPressed = isShift,
            IsCtrlPressed = isCtrl,
            IsAltPressed = isAlt,
            Keypressed = key
        };

        var result = keypress.BuildKeypressText();

        Assert.Equal(expected, result);
    }
}
