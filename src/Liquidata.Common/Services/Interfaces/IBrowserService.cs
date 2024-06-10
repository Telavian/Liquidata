using Liquidata.Common.Actions.Enums;
using Liquidata.Common.Models;

namespace Liquidata.Common.Services.Interfaces;

public interface IBrowserService
{        
    Task InitializeBrowserAsync();
    Task<bool> CheckIfWebSecurityEnabledAsync();
    Task<bool> WaitForBrowserReadyAsync(TimeSpan waitTime);
    Task<SelectionInfo> GetSelectionInfoAsync(string xpath);
    Task<string[]> GetAllMatchesAsync(string xpath);

    Task<bool> ExecuteJavascriptAsync(string script);
    Task<(bool success, T result)> ExecuteJavascriptAsync<T>(string script);
    Task StoreDataAsync(string name, string value, StoreType storeType);
    Task<IBrowserService> ClickOpenInNewPageAsync(string selection, ClickButton clickButton, bool isDoubleClick);
    Task ClickSelectionAsync(string selection, ClickButton clickButton, bool isDoubleClick);
    Task<string> GetVariableAsync(string name);
    Task SetVariableAsync(string name, string value);
    Task RemoveVariableAsync(string name);
    Task HoverSelectionAsync(string selection);
    Task InputToSelectionAsync(string selection, string value);
    Task KeypressToSelectionAsync(string currentSelection, bool isShiftPressed, bool isCtrlPressed, bool isAltPressed, string keypressed);
    Task ReloadPageAsync();
    Task<byte[]> GetScreenshotAsync();
    Task ScrollPageAsync(ScrollType scrollType);
    Task SolveCaptchaAsync();
}
